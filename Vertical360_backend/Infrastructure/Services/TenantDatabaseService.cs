using Microsoft.EntityFrameworkCore;
using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Infrastructure.Persistence;

namespace Vertical360_backend.Infrastructure.Services
{
    public class TenantDatabaseService : ITenantDatabaseService
    {
        private readonly IConfiguration _configuration;

        public TenantDatabaseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<DbContext> GetTenantDbContextAsync(string tenantId)
        {
            var baseConn = _configuration.GetConnectionString("TenantBase")
                ?? throw new InvalidOperationException("ConnectionString TenantBase no encontrada.");

            // Ejemplo: reemplazar placeholder con el tenantId
            var connString = baseConn.Replace("{tenant}", tenantId);

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseMySql(connString, ServerVersion.AutoDetect(connString));

            // provide a non-null IHttpContextAccessor to avoid NRE in SaveChangesAsync
            var dbContext = new ApplicationDbContext(optionsBuilder.Options, new HttpContextAccessor());
            await dbContext.Database.EnsureCreatedAsync();
            return dbContext;
        }

        public async Task CreateTenantDatabaseAsync(string tenantId)
        {
            var baseConn = _configuration.GetConnectionString("TenantBase")
                ?? throw new InvalidOperationException("ConnectionString TenantBase no encontrada.");

            var connString = baseConn.Replace("{tenant}", tenantId);

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseMySql(connString, ServerVersion.AutoDetect(connString));

            var dbContext = new ApplicationDbContext(optionsBuilder.Options, new HttpContextAccessor());
            await dbContext.Database.EnsureCreatedAsync();
        }
    }
}
