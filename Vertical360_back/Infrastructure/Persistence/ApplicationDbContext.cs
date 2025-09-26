using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Vertical360_back.Application.UseCases;
using Vertical360_back.Common;
using Vertical360_back.Domain.Entityes;

namespace Vertical360_back.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    private string? tenantId;

    // Constructor runtime con IServiceTenant
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IServiceTenant serviceTenant)
        : base(options)
    {
        tenantId = serviceTenant?.GetTenant();
    }

    // Constructor solo para EF Core en tiempo de diseño
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        tenantId = null;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(tenantId))
        {
            foreach (var item in ChangeTracker.Entries().Where(e => e.State == EntityState.Added
                && e.Entity is IEntityTenant))
            {
                if (string.IsNullOrEmpty(tenantId))
                {
                    throw new Exception("Tenant Id no encontrado al momento de crear el registro");
                }

                var entity = item.Entity as IEntityTenant;
                entity!.TenantId = tenantId;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CompanyUserPermissions>().HasKey(x => new { x.CompanyId, x.UserId, x.Permissions });

        builder.Entity<Countries>().HasData(
            new Countries { Code = "ARG", Name = "Argentina", Id = Guid.NewGuid() },
            new Countries { Code = "BRA", Name = "Brasil", Id = Guid.NewGuid() },
            new Countries { Code = "CHL", Name = "Chile", Id = Guid.NewGuid() },
            new Countries { Code = "COL", Name = "Colombia", Id = Guid.NewGuid() }
        );

        if (!string.IsNullOrEmpty(tenantId))
        {
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                var type = entity.ClrType;

                if (typeof(IEntityTenant).IsAssignableFrom(type))
                {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(ConstructFilterGlobalTenant), BindingFlags.NonPublic | BindingFlags.Static)
                        ?.MakeGenericMethod(type);

                    var filter = method?.Invoke(null, new object[] { this })!;
                    entity.SetQueryFilter((LambdaExpression)filter);
                    entity.AddIndex(entity.FindProperty(nameof(IEntityTenant.TenantId))!);
                }
                else if (type.NotValidationTenant())
                {
                    continue;
                }
                else
                {
                    throw new Exception($"La entidad {entity} no ha sido marcada como tenant o común");
                }
            }
        }

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTimeOffset) || property.ClrType == typeof(DateTimeOffset?))
                {
                    property.SetColumnType("datetime(6)");
                }
            }
        }
    }

    private static LambdaExpression ConstructFilterGlobalTenant<TEntity>(
        ApplicationDbContext context)
        where TEntity : class, IEntityTenant
    { 
        Expression<Func<TEntity, bool>> filter = e => e.TenantId == context.tenantId;
        return filter;
    }

    // DbSets
    public DbSet<Products> Products => Set<Products>();
    public DbSet<Countries> Countries => Set<Countries>();
    public DbSet<Companies> Companies => Set<Companies>();
    public DbSet<CompanyUserPermissions> CompanyUserPermissions => Set<CompanyUserPermissions>();
    public DbSet<LinkUserCompany> LinkUsersCommpany => Set<LinkUserCompany>();
}
