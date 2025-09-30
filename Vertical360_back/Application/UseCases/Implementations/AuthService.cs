using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;
using Vertical360_back.Application.Contracts.Auth;
using Vertical360_back.Application.Interfaces.Services;

namespace Vertical360_back.Application.UseCases.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<LoginResultDto> LoginAsync(string email, string password, bool rememberMe)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return new LoginResultDto
                {
                    Success = false,
                    ErrorMessage = "Usuario o contraseña incorrectos"
                };
            }

            var user = await _userManager.FindByEmailAsync(email);

            return new LoginResultDto
            {
                Success = true,
                UserId = user!.Id,
                RedirectUrl = "/home"
            };
        }
    }
}
