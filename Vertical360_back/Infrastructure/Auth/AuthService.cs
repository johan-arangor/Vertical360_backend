using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vertical360_back.Application.Contracts.Auth;
using Vertical360_back.Application.Interfaces.Services;
using Vertical360_back.Domain.Enums;
using Vertical360_back.Infrastructure.Persistence;

namespace Vertical360_back.Infrastructure.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IServiceChangeTenant _serviceChangeTenant;
        private readonly ApplicationDbContext _context;

        public AuthService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IServiceChangeTenant serviceChangeTenant,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _serviceChangeTenant = serviceChangeTenant;
            _context = context;
        }

        public async Task<LoginResultDto> LoginAsync(string email, string password, bool rememberMe)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, false);

            if (!result.Succeeded)
            {
                return new LoginResultDto { Success = false, ErrorMessage = "Usuario o contraseña incorrectos" };
            }

            var user = await _userManager.FindByEmailAsync(email);

            var companiesLink = await _context.CompanyUserPermissions
                .Where(x => x.UserId == user!.Id && x.Permissions == Permissions.Null)
                .OrderBy(x => x.CompanyId)
                .Take(2)
                .Select(x => x.CompanyId)
                .ToListAsync();

            if (companiesLink.Count == 0)
            {
                return new LoginResultDto { Success = true, UserId = user!.Id, RedirectUrl = "/Home/Index" };
            }
            else if (companiesLink.Count == 1)
            {
                await _serviceChangeTenant.ReplaceTenant(companiesLink[0], user!.Id);
                return new LoginResultDto { Success = true, UserId = user!.Id, RedirectUrl = "/Home/Index" };
            }
            else
            {
                return new LoginResultDto { Success = true, UserId = user!.Id, RedirectUrl = "/Companies/Change" };
            }
        }
    }
}
