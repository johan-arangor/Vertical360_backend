using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Security.Claims;
using Vertical360_backend.Application.DTOs.Auth;
using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Domain.Entities;
using Vertical360_backend.Infrastructure.Persistence;
using Vertical360_backend.Infrastructure.Services;

namespace Vertical360_backend.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
            ApplicationDbContext context, ITokenService tokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<LoginResultDto> LoginAsync(LoginRequestDto model)
        {
            var userName = model.UserName?.Trim();
            var password = model.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrEmpty(password))
                throw new UnauthorizedAccessException("UserName y Password son requeridos.");
            // Buscar usuario por UserName
            var user = await _userManager.FindByNameAsync(userName);
            
            if (user == null) throw new UnauthorizedAccessException("Usuario no encontrado.");
            // Validar contraseña
            var signIn = await _signInManager.PasswordSignInAsync(userName, password, model.RememberMe, false);
            
            if (!signIn.Succeeded) throw new UnauthorizedAccessException("Credenciales inválidas.");
            // obtener compañías asociadas
            var links = await _context.LinkUsersCompany
                .Where(l => l.UserId == user.Id)
                .ToListAsync();
            var companyIds = links.Select(l => l.CompanyId).ToList();
            var companies = await _context.Companies
                .Where(c => companyIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id);  // evita N+1 queries
            // Clientes asociados al usuario
            var associatedClients = links.Select(l =>
            {
                var company = _context.Companies.Find(l.CompanyId);
                return new ClientInfoDto
                {
                    Id = l.CompanyId,
                    Name = company?.Name ?? string.Empty
                };
            }).ToList();
            // Generar token para la compañía seleccionada usando tenantKey
            var preAuthToken = await _tokenService.GeneratePreAuthTokenAsync(user);
            // Si no se pidió companyId, devolvemos la lista y no generamos token
            return new LoginResultDto
            {
                Success = true,
                Token = preAuthToken,
                AssociatedClients = associatedClients
            };
        }

        public async Task<LoginResultDto> SelectTenantAsync(SelectTenantRequestDto model)
        {
            if (!Guid.TryParse(model.CompanyId, out var companyId)) throw new UnauthorizedAccessException("CompanyId inválido.");
            // Validar y extraer claims del token intermedio
            var principal = _tokenService.ValidatePreAuthToken(model.PreAuthToken);
            // Validar el tipo de token
            var tokenType = principal.FindFirstValue("token_type");
            
            if (tokenType != "pre_auth")
                throw new UnauthorizedAccessException("Token inválido.");
            // Extraer el usuario logeado desde el token
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Token inválido.");
            // Validar el usuario en la base de nuevo
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null) throw new UnauthorizedAccessException("Usuario no encontrado.");
            // Extraer el tenant id del usuario logeado
            var link = await _context.LinkUsersCompany
                .FirstOrDefaultAsync(l => l.UserId == userId && l.CompanyId == companyId);
            
            if (link == null) throw new UnauthorizedAccessException("El usuario no está vinculado a esta compañía.");
            // Obtener tenantKey de la compañía
            var company = await _context.Companies.FindAsync(companyId);
            
            if (company == null) throw new Exception("Compañía no encontrada.");

            var tenantKey = company.TenantKey ?? string.Empty;
            var token = await _tokenService.GenerateTokenAsync(user, companyId, tenantKey);
            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? string.Empty;

            return new LoginResultDto
            {
                Success = true,
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                Role = primaryRole,
                Token = token
            };
        }
    }
}
