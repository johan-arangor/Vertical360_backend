namespace Vertical360.Application.Interfaces
{
    /// <summary>
    /// Fábrica que resuelve el TenantDbContext correcto a partir del tenantId activo en la petición.
    /// Los servicios de dominio del tenant deben recibirla por DI en lugar de recibir el DbContext directamente.
    /// La implementación concreta reside en Infrastructure; esta interfaz permanece en Application
    /// para mantener la inversión de dependencias (Ports & Adapters).
    /// </summary>
    public interface ITenantDbContextFactory
    {
        /// <summary>
        /// Retorna el contexto EF Core del tenant activo en el request HTTP actual.
        /// Lanza <see cref="InvalidOperationException"/> si no hay tenant resuelto.
        /// El tipo de retorno es <see cref="Microsoft.EntityFrameworkCore.DbContext"/> para
        /// mantener Application libre de referencias concretas de Infrastructure.
        /// </summary>
        Microsoft.EntityFrameworkCore.DbContext CreateContext();
    }
}
