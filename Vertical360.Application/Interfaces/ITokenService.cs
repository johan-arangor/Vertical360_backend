using System.Security.Claims;
using Vertical360.Core.Entities;

namespace Vertical360.Application.Interfaces
{
    public interface ITokenService
    {
        // Genera token incluyendo claim del tenant. tenantKey es la clave del tenant (Companies.TenantKey).
        Task<string> GenerateTokenAsync(ApplicationUser user, Guid companyId, string tenantKey);
        Task<string> GeneratePreAuthTokenAsync(ApplicationUser user);
        ClaimsPrincipal ValidatePreAuthToken(string token);
    }
}
