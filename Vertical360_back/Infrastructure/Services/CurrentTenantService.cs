using System.Security.Claims;
using Vertical360_back.Application.Interfaces.Services;

namespace Vertical360_back.Infrastructure.Services
{
    public class CurrentTenantService : ICurrentTenantService
    {
        public string TenantId { get; }
        public string? UserId { get; }
        public bool HasTenant => !string.IsNullOrEmpty(TenantId);

        public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (user != null)
            {
                // Recuperar TenantId del claim (agregado al autenticarse)
                TenantId = user.FindFirst("TenantId")?.Value ?? string.Empty;
                // Recuperar Id del usuario autenticado
                UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
        }
    }
}
