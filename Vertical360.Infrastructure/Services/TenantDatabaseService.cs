using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Vertical360.Application.Interfaces;
using Vertical360.Infrastructure.Persistence;

namespace Vertical360.Infrastructure.Services
{
    /// <summary>
    /// Gestiona el ciclo de vida de las bases de datos de cada tenant:
    /// creación, migración y eliminación (rollback).
    /// </summary>
    public class TenantDatabaseService : ITenantDatabaseService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TenantDatabaseService> _logger;

        public TenantDatabaseService(
            IConfiguration configuration,
            ILogger<TenantDatabaseService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<DbContext> GetTenantDbContextAsync(string tenantId)
        {
            var options = BuildOptions(tenantId);
            return new TenantDbContext(options);
        }

        public async Task CreateTenantDatabaseAsync(string tenantId)
        {
            _logger.LogInformation("Creando base de datos para tenant: {TenantId}", tenantId);
            var options = BuildOptions(tenantId);
            await using var context = new TenantDbContext(options);
            await context.Database.EnsureCreatedAsync();
            _logger.LogInformation("Base de datos creada para tenant: {TenantId}", tenantId);
        }

        public async Task DropTenantDatabaseAsync(string tenantId)
        {
            _logger.LogWarning("Eliminando base de datos del tenant: {TenantId} (rollback)", tenantId);
            var options = BuildOptions(tenantId);
            await using var context = new TenantDbContext(options);
            await context.Database.EnsureDeletedAsync();
            _logger.LogWarning("Base de datos eliminada para tenant: {TenantId}", tenantId);
        }

        private DbContextOptions<TenantDbContext> BuildOptions(string tenantId)
        {
            var serverBase = _configuration.GetConnectionString("ServerBase")
                ?? throw new InvalidOperationException("ConnectionString 'ServerBase' no encontrada.");

            var dbName = $"v360_tenant_{tenantId}";
            var connString = $"{serverBase}Database={dbName};";

            return new DbContextOptionsBuilder<TenantDbContext>()
                .UseMySql(connString, ServerVersion.AutoDetect(connString),
                    mySql => mySql.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null))
                .Options;
        }
    }
}
