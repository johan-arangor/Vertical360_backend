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

        public DbSet<Companies> Companies => Set<Companies>();
        public DbSet<Resident> Residents => Set<Resident>();
        public DbSet<LinkUserCompany> LinkUsersCompany => Set<LinkUserCompany>();

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
