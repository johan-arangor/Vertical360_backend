using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Vertical360_back.Infrastructure.Persistence
{
    public class CommonDbContextFactory : IDesignTimeDbContextFactory<CommonDbContext>
    {
        public CommonDbContext CreateDbContext(string[] args)
        {
            // Obtener la configuración del proyecto (appsettings.json)
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Obtener la cadena de conexión
            var connectionString = configuration.GetConnectionString("CommonConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'CommonConnection' no se encontró en appsettings.json.");
            }

            // Configurar DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<CommonDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            // Crear la instancia de CommonDbContext
            return new CommonDbContext(optionsBuilder.Options);
        }
    }
}
