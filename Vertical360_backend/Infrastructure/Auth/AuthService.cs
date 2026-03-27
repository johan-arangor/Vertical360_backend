using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vertical360_backend.Application.DTOs.Auth;
using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Domain.Entities;
using Vertical360_backend.Infrastructure.Persistence;

namespace Vertical360_backend.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MasterDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            MasterDbContext context,
            ITokenService tokenService)
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
            Console.WriteLine($"UserName: {userName}");
            Console.WriteLine($"Password: {password}");
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrEmpty(password))
                throw new UnauthorizedAccessException("UserName y Password son requeridos.");

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) throw new UnauthorizedAccessException("Usuario no encontrado.");

            var signIn = await _signInManager.PasswordSignInAsync(userName, password, model.RememberMe, false);
            if (!signIn.Succeeded) throw new UnauthorizedAccessException("Credenciales inválidas.");

            // Carga los vínculos y las compañías en una sola consulta — sin N+1
            var links = await _context.LinkUsersCompany
                .Where(l => l.UserId == user.Id)
                .Include(l => l.Company)
                .ToListAsync();

            var associatedClients = links
                .Where(l => l.Company != null)
                .Select(l => new ClientInfoDto
                {
                    Id = l.CompanyId,
                    Name = l.Company!.Name
                })
                .ToList();

            var preAuthToken = await _tokenService.GeneratePreAuthTokenAsync(user);

            return new LoginResultDto
            {
                Success = true,
                Token = preAuthToken,
                AssociatedClients = associatedClients
            };
        }

        public async Task<LoginResultDto> SelectTenantAsync(SelectTenantRequestDto model)
        {
            if (!Guid.TryParse(model.CompanyId, out var companyId))
                throw new UnauthorizedAccessException("CompanyId inválido.");

            var principal = _tokenService.ValidatePreAuthToken(model.PreAuthToken);

            var tokenType = principal.FindFirstValue("token_type");
            if (tokenType != "pre_auth")
                throw new UnauthorizedAccessException("Token inválido.");

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("Token inválido.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new UnauthorizedAccessException("Usuario no encontrado.");

            var link = await _context.LinkUsersCompany
                .Include(l => l.Company)
                .FirstOrDefaultAsync(l => l.UserId == userId && l.CompanyId == companyId);

            if (link == null)
                throw new UnauthorizedAccessException("El usuario no está vinculado a esta compañía.");

            if (link.Company == null)
                throw new Exception("Compañía no encontrada.");

            var tenantKey = link.Company.TenantKey ?? string.Empty;
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
