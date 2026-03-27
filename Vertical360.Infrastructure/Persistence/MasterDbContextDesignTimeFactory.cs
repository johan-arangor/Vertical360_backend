using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Vertical360.Infrastructure.Persistence
{
    /// <summary>
    /// Fábrica de design-time para que el CLI de EF Core pueda instanciar
    /// MasterDbContext al generar o aplicar migraciones sin necesidad del host.
    ///
    /// Uso:
    ///   dotnet ef migrations add NombreMigracion --context MasterDbContext
    ///   dotnet ef database update --context MasterDbContext
    /// </summary>
    public class MasterDbContextDesignTimeFactory : IDesignTimeDbContextFactory<MasterDbContext>
    {
        public MasterDbContext CreateDbContext(string[] args)
        {
            // Lee la cadena de conexión desde appsettings.json en tiempo de diseño
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connString = config.GetConnectionString("SharedDb")
                ?? throw new InvalidOperationException(
                    "ConnectionString 'SharedDb' no encontrada en appsettings.json. " +
                    "Requerida para generar migraciones de MasterDbContext.");

            var options = new DbContextOptionsBuilder<MasterDbContext>()
                .UseMySql(connString, ServerVersion.AutoDetect(connString))
                .Options;

            return new MasterDbContext(options);
        }
    }
}
