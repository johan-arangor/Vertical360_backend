using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vertical360_backend.Domain.Entities;
using Vertical360_backend.Domain.ValueObjects;

namespace Vertical360_backend.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // DbSets de las entidades
        public DbSet<Companies> Companies => Set<Companies>();
        public DbSet<Resident> Residents => Set<Resident>();
        public DbSet<LinkUserCompany> LinkUsersCompany => Set<LinkUserCompany>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<City> Cities => Set<City>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Ignore<ApplicationUserRole>();

            builder.Entity<LinkUserCompany>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.CompanyId });
                entity.Property(e => e.RoleName).IsRequired();

                // Relaciones opcionales:
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Company)
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Resident>().HasIndex(r => new { r.TenantId, r.Document }).IsUnique();

            // Configuración de Country/Department/City
            builder.Entity<Country>(c =>
            {
                c.HasKey(x => x.Id);
                c.Property(x => x.Name).IsRequired();
                c.Property(x => x.Acronym).HasMaxLength(10);
                c.Property(x => x.Code).HasMaxLength(20);
                c.HasIndex(x => x.Name).IsUnique();
            });

            builder.Entity<Department>(d =>
            {
                d.HasKey(x => x.Id);
                d.Property(x => x.Name).IsRequired();
                d.Property(x => x.Acronym).HasMaxLength(10);
                d.Property(x => x.Code).HasMaxLength(20);
                d.HasIndex(x => new { x.CountryId, x.Name }).IsUnique();
                d.HasOne(x => x.Country)
                 .WithMany(c => c.Departments)
                 .HasForeignKey(x => x.CountryId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<City>(ct =>
            {
                ct.HasKey(x => x.Id);
                ct.Property(x => x.Name).IsRequired();
                ct.Property(x => x.Acronym).HasMaxLength(10);
                ct.Property(x => x.Code).HasMaxLength(20);
                ct.HasIndex(x => new { x.DepartmentId, x.Name }).IsUnique();
                ct.HasOne(x => x.Department)
                  .WithMany(d => d.Cities)
                  .HasForeignKey(x => x.DepartmentId)
                  .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // auto set TenantId for tenant-scoped entities if available in claim
            var tenant = _httpContextAccessor?.HttpContext?.User?.FindFirst(Constants.CLAIM_TENANT)?.Value;

            if (!string.IsNullOrEmpty(tenant))
            {
                foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added && e.Entity is Resident))
                {
                    if (entry.Entity is Resident r && string.IsNullOrEmpty(r.TenantId))
                    {
                        r.TenantId = tenant;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
