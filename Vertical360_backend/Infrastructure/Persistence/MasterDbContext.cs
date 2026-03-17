using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vertical360_backend.Application.DTOs.Auth;
using Vertical360_backend.Domain.Entities;

namespace Vertical360_backend.Infrastructure.Persistence
{
    /// <summary>
    /// Contexto de la base de datos maestra (SharedDb).
    /// Contiene: Identity (usuarios, roles), Companies, LinkUserCompany,
    /// PasswordResetOtps y datos geográficos (países, departamentos, ciudades).
    /// NO contiene entidades propias de cada tenant.
    /// </summary>
    public class MasterDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options)
            : base(options)
        {
        }

        public DbSet<PasswordResetOtp> PasswordResetOtps => Set<PasswordResetOtp>();
        public DbSet<Companies> Companies => Set<Companies>();
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

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Company)
                      .WithMany()
                      .HasForeignKey(e => e.CompanyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

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
    }
}
