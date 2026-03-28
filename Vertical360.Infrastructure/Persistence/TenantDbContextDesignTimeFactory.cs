using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Vertical360.Infrastructure.Persistence
{
    /// <summary>
    /// Fábrica de tiempo de diseño para TenantDbContext.
    /// Permite a las herramientas de EF Core (dotnet ef) generar migraciones para el esquema multitenant.
    /// </summary>
    public class TenantDbContextDesignTimeFactory : IDesignTimeDbContextFactory<TenantDbContext>
    {
        public TenantDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                // Hacemos que appsettings sea opcional para que no falle al ejecutarse
                // desde carpetas que no sean el proyecto principal (como Vertical360.Infrastructure)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Valores por defecto para desarrollo si no se encuentra el connection string
            var serverBase = config.GetConnectionString("ServerBase") 
                ?? "Server=localhost;Uid=root;Pwd=root;";
            
            // Usamos un nombre de BD de "plantilla" para generar las migraciones del esquema
            var connString = $"{serverBase}Database=v360_tenant_template;";

            var options = new DbContextOptionsBuilder<TenantDbContext>()
                .UseMySql(connString, ServerVersion.AutoDetect(connString),
                    mySql => mySql.EnableRetryOnFailure())
                .Options;

            return new TenantDbContext(options);
        }
    }
}
