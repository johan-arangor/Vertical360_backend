using System.Security.Claims;
using Vertical360_backend.Domain.Entities;

namespace Vertical360_backend.Application.Interfaces
{
    public interface ITokenService
    {
        // Genera token incluyendo claim del tenant. tenantKey es la clave del tenant (Companies.TenantKey).
        Task<string> GenerateTokenAsync(ApplicationUser user, Guid companyId, string tenantKey);
        Task<string> GeneratePreAuthTokenAsync(ApplicationUser user);
        ClaimsPrincipal ValidatePreAuthToken(string token);
    }
}
