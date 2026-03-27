using Microsoft.EntityFrameworkCore;
using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Infrastructure.Persistence;

namespace Vertical360_backend.Infrastructure.Services
{
    /// <summary>
    /// Resuelve dinámicamente el <see cref="TenantDbContext"/> para el tenant
    /// activo en la petición HTTP. Construye el connection string reemplazando
    /// el placeholder {tenant} con el TenantId obtenido de <see cref="ICurrentTenantService"/>.
    /// </summary>
    public class TenantDbContextFactory : ITenantDbContextFactory
    {
        private readonly ICurrentTenantService _currentTenant;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantDbContextFactory(
            ICurrentTenantService currentTenant,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _currentTenant = currentTenant;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public TenantDbContext CreateContext()
        {
            var tenantId = _currentTenant.TenantId;

            if (string.IsNullOrEmpty(tenantId))
                throw new InvalidOperationException(
                    "No hay un tenant activo. Asegúrese de que el JWT contiene el claim 'tenant_id'.");

            var serverBase = _configuration.GetConnectionString("ServerBase")
                ?? throw new InvalidOperationException("ConnectionString 'ServerBase' no encontrada.");

            // ServerBase debe terminar sin nombre de base de datos:
            // "Server=localhost;Uid=root;Pwd=root;"
            // El nombre de la BD del tenant se construye con el prefijo + tenantId.
            var dbName = $"v360_tenant_{tenantId}";
            var connString = $"{serverBase}Database={dbName};";

            var options = new DbContextOptionsBuilder<TenantDbContext>()
                .UseMySql(connString, ServerVersion.AutoDetect(connString),
                    mySql => mySql.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null))
                .Options;

            return new TenantDbContext(options, _httpContextAccessor);
        }
    }
}
