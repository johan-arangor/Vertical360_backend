using Microsoft.EntityFrameworkCore;

namespace Vertical360.Infrastructure.Persistence
{
    /// <summary>
    /// Alias de compatibilidad para las migraciones de EF Core existentes.
    /// Las migraciones actuales fueron generadas contra este tipo.
    /// Todo código de aplicación debe inyectar <see cref="MasterDbContext"/>.
    ///
    /// NOTA: Al generar nuevas migraciones usar:
    ///   dotnet ef migrations add NombreMigracion --context MasterDbContext
    /// </summary>
    [Obsolete("Usar MasterDbContext en servicios y repositorios. Este tipo existe solo para compatibilidad con migraciones existentes.")]
    public class ApplicationDbContext : MasterDbContext
    {
        public ApplicationDbContext(DbContextOptions<MasterDbContext> options)
            : base(options)
        {
        }
    }
}

