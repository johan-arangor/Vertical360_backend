using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            var signIn = await _signInManager.PasswordSignInAsync(model.Document, model.Password, model.RememberMe, false);
            if (!signIn.Succeeded) throw new UnauthorizedAccessException("Credenciales inválidas.");

            var user = await _userManager.FindByNameAsync(model.Document);
            if (user == null) throw new UnauthorizedAccessException("Usuario no encontrado.");

            // obtener compañías asociadas y escoger una por defecto (si existe)
            var links = await _context.LinkUsersCompany.Where(l => l.UserId == user.Id).ToListAsync();
            var result = new LoginResultDto
            {
                Success = true,
                UserId = user.Id,
                AssociatedClients = links.Select(l => new ClientInfoDto { Id = l.CompanyId, Name = _context.Companies.Find(l.CompanyId)!.Name }).ToList()
            };

            return result;
        }
    }
}
