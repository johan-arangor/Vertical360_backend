namespace Vertical360_backend.Application.Interfaces
{
    /// <summary>
    /// Fábrica que resuelve el <see cref="Infrastructure.Persistence.TenantDbContext"/>
    /// correcto a partir del tenantId activo en la petición.
    /// Los servicios de dominio del tenant deben recibirla por DI
    /// en lugar de recibir el DbContext directamente.
    /// </summary>
    public interface ITenantDbContextFactory
    {
        /// <summary>
        /// Retorna el TenantDbContext para el tenant activo en el request.
        /// Lanza <see cref="InvalidOperationException"/> si no hay tenant resuelto.
        /// </summary>
        Infrastructure.Persistence.TenantDbContext CreateContext();
    }
}
