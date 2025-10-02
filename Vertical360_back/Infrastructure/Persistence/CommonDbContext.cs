using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vertical360_back.Domain.Entityes;
using Vertical360_back.Domain.Entityes.Common;

namespace Vertical360_back.Infrastructure.Persistence
{
    public class CommonDbContext : IdentityDbContext<IdentityUser>
    {
        public CommonDbContext(DbContextOptions<CommonDbContext> options) : base(options) { }

        // Tablas Comunes: Companies y Countries.
        // Companies es común para mapear el TenantId a su nombre de BD.
        public DbSet<Companies> Companies => Set<Companies>();
        public DbSet<Countries> Countries => Set<Countries>();
        public DbSet<Departments> Departments => Set<Departments>();
        public DbSet<Cities> Cities => Set<Cities>();
        public DbSet<LinkUserCompany> LinkUsersCommpany => Set<LinkUserCompany>();
        public DbSet<CompanyUserPermissions> CompanyUserPermissions => Set<CompanyUserPermissions>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Departments>()
                .HasOne(d => d.Country)
                .WithMany(c => c.Departments)
                .HasForeignKey(d => d.CountryId);

            modelBuilder.Entity<Cities>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Cities)
                .HasForeignKey(c => c.DepartmentId);

            modelBuilder.Entity<CompanyUserPermissions>()
                .HasKey(x => new { x.CompanyId, x.UserId, x.Permissions });

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                
                if (!string.IsNullOrEmpty(tableName) && !tableName.StartsWith("AspNet"))
                {
                    modelBuilder.Entity(entityType.Name).ToTable(tableName);
                }

                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTimeOffset) || property.ClrType == typeof(DateTimeOffset?))
                    {
                        property.SetColumnType("datetime(6)");
                    }
                }
            }
        }
    }
}
