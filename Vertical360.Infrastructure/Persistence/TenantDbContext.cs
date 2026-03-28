using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Vertical360.Core.Entities;
using Vertical360.Core.ValueObjects;

namespace Vertical360.Infrastructure.Persistence
{
    /// <summary>
    /// Contexto de la base de datos de cada tenant (unidad residencial).
    /// Contiene únicamente entidades propias del negocio de la copropiedad.
    /// Se instancia dinámicamente por <see cref="ITenantDbContextFactory"/>
    /// usando el connection string del tenant activo.
    /// </summary>
    public class TenantDbContext : DbContext
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public TenantDbContext(
            DbContextOptions<TenantDbContext> options,
            IHttpContextAccessor? httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Resident> Residents => Set<Resident>();
        public DbSet<HousingUnit> HousingUnits => Set<HousingUnit>();
        public DbSet<Visit> Visits => Set<Visit>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Resident>(r =>
            {
                r.HasIndex(x => new { x.TenantId, x.Document }).IsUnique();
                r.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
                r.Property(x => x.LastName).IsRequired().HasMaxLength(100);
                r.Property(x => x.Document).IsRequired().HasMaxLength(20);
                r.Property(x => x.Email).IsRequired().HasMaxLength(200);
                r.Property(x => x.PhoneNumber).HasMaxLength(20);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Auto-asigna TenantId en entidades de tipo Resident al momento de crear
            var tenantId = _httpContextAccessor?.HttpContext?.User
                ?.FindFirst(Constants.CLAIM_TENANT)?.Value;

            if (!string.IsNullOrEmpty(tenantId))
            {
                foreach (var entry in ChangeTracker.Entries<Resident>()
                    .Where(e => e.State == EntityState.Added && string.IsNullOrEmpty(e.Entity.TenantId)))
                {
                    entry.Entity.TenantId = tenantId;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
