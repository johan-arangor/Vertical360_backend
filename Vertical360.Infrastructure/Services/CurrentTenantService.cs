using Vertical360.Application.Interfaces;

namespace Vertical360.Infrastructure.Services
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
