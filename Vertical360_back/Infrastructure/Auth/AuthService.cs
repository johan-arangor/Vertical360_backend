using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vertical360_back.Application.Contracts.Auth;
using Vertical360_back.Application.Interfaces.Services;
using Vertical360_back.Infrastructure.Persistence;

namespace Vertical360_back.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;

        public AuthService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ITokenService tokenService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _context = context;
        }

        public async Task<LoginResultDto> LoginAsync(string email, string password, bool rememberMe)
        {
            var resultDto = new LoginResultDto();
            // Autenticar el usuario usando ASP.NET Identity
            var signInResult = await _signInManager.PasswordSignInAsync(email, password, rememberMe, false);

            if (!signInResult.Succeeded)
            {
                throw new UnauthorizedAccessException("Credenciales invalidas o cuenta no confirmada.");
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new UnauthorizedAccessException("No se ha encontrado el objeto de usuario.");
            }
            // Gerar el JWT
            resultDto.Token = await _tokenService.GenerateTokenAsync(user);
            // Validar rol para determinar ruta del usuario
            const string SuperAdminRole = "PlatformSuperAdmin";
            bool isSuperAdmin = await _userManager.IsInRoleAsync(user, SuperAdminRole);

            if (isSuperAdmin)
            {
                // Si es SuperAdmin de la Plataforma, redirigir a la interfaz de administración
                resultDto.RedirectUrl = "home/admin";
            }
            else
            {
                // Si es un usuario normal (tenant user), redirigir a la selección de rol/compañía
                resultDto.RedirectUrl = "home/selected-role";
            }
            // Obtener las compañías (Clientes) asociadas a este usuario
            var associatedClients = await _context.LinkUsersCompany
                .Where(link => link.UserId == user.Id)
                .Join(
                    _context.Companies,
                    link => link.CompanyId,
                    company => company.Id,
                    (link, company) => new ClientInfoDTO
                    {
                        CompanyId = company.Id,
                        Name = company.Name,
                    })
                .ToListAsync();

            resultDto.Success = true;
            resultDto.Message = "Login correcto.";
            resultDto.UserId = user.Id;
            resultDto.AssociatedClients = associatedClients;

            return resultDto;
        }
    }
}
