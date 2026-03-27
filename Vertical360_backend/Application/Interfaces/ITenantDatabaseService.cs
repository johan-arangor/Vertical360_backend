using Microsoft.EntityFrameworkCore;

namespace Vertical360_backend.Application.Interfaces
{
    public interface ITenantDatabaseService
    {
        Task<DbContext> GetTenantDbContextAsync(string tenantId);
        Task CreateTenantDatabaseAsync(string tenantId);

        /// <summary>
        /// Elimina la base de datos del tenant. Se usa como rollback
        /// si la creación de la compañía falla después de haber creado la BD.
        /// </summary>
        Task DropTenantDatabaseAsync(string tenantId);
    }
}
