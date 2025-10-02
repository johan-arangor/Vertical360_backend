using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Vertical360_back.Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Usamos la plantilla y le asignamos un nombre de BD falso para el diseño.
            var template = configuration.GetConnectionString("TenantConnectionTemplate");

            if (string.IsNullOrEmpty(template))
            {
                throw new InvalidOperationException("La cadena de conexión 'TenantConnectionTemplate' no se encontró en appsettings.json.");
            }

            // Nombre falso que permite generar la migración
            var tempDbName = "temp_design_time_db";
            var connectionString = string.Format(template, tempDbName);

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            // Crear la instancia de ApplicationDbContext
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
