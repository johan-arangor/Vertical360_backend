using Microsoft.AspNetCore.Identity;

namespace Vertical360_back.Application.Interfaces.Services
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(IdentityUser user);
    }
}
