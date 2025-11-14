using Microsoft.EntityFrameworkCore;

namespace Vertical360_backend.Application.Interfaces
{
    public interface ITenantDatabaseService
    {
        Task<DbContext> GetTenantDbContextAsync(string tenantId);
        Task CreateTenantDatabaseAsync(string tenantId);
    }
}
