namespace Vertical360.Application.Interfaces
{
    /// <summary>
    /// Define las operaciones de ciclo de vida de la base de datos de un tenant.
    /// Pertenece a la capa Application (contrato puro, sin dependencias de infraestructura).
    /// </summary>
    public interface ITenantDatabaseService
    {
        /// <summary>
        /// Crea la base de datos del tenant identificado por <paramref name="tenantId"/>.
        /// </summary>
        Task CreateTenantDatabaseAsync(string tenantId);

        /// <summary>
        /// Elimina la base de datos del tenant. Se usa como rollback
        /// si la creación de la compañía falla después de haber creado la BD.
        /// </summary>
        Task DropTenantDatabaseAsync(string tenantId);
    }
}
