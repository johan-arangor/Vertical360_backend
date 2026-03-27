using Vertical360_backend.Application.Interfaces;

namespace Vertical360_backend.Infrastructure.Services
{
    public class CurrentTenantService : ICurrentTenantService
    {
        public string? TenantId { get; private set; }

        public Task SetTenantAsync(string tenantId)
        {
            TenantId = tenantId;
            return Task.CompletedTask;
        }
    }
}
