using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Vertical360_back.Application.Interfaces.Services;
using Vertical360_back.Domain.Entityes;
using Vertical360_back.Domain.Entityes.Common;
using Vertical360_back.Domain.Entityes.Tenants;
using Vertical360_back.Domain.ValueObjects;

namespace Vertical360_back.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    private readonly IServiceUser _serviceUser;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IServiceUser serviceUser, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _serviceUser = serviceUser;
        _httpContextAccessor = httpContextAccessor;
    }

    // Constructor de diseño (para migraciones)
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    #region DbSets
    public DbSet<Companies> Companies => Set<Companies>();
    public DbSet<Countries> Countries => Set<Countries>();
    public DbSet<Departments> Departments => Set<Departments>();
    public DbSet<Cities> Cities => Set<Cities>();
    public DbSet<Connection> Connections => Set<Connection>();
    public DbSet<LinkUserCompany> LinkUsersCompany => Set<LinkUserCompany>();
    public DbSet<CompanyUserPermissions> CompanyUserPermissions => Set<CompanyUserPermissions>();
    public DbSet<Resident> Residents => Set<Resident>();
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica convenciones globales
        ApplyDateTimeOffsetConvention(modelBuilder);

        // Aplicar configuración de entidades por ensamblado
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Filtro global por TenantId
        ApplyTenantFilter(modelBuilder);
    }

    private static void ApplyDateTimeOffsetConvention(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTimeOffset) || property.ClrType == typeof(DateTimeOffset?))
                    property.SetColumnType("datetime(6)");
            }
        }
    }

    private void ApplyTenantFilter(ModelBuilder modelBuilder)
    {
        var tenantId = _httpContextAccessor.HttpContext?.User?.FindFirst(Constants.CLAIM_TENANT)?.Value;

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IEntityTenant).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(null, new object[] { modelBuilder, tenantId });
            }
        }
    }

    private static void SetTenantFilter<TEntity>(ModelBuilder modelBuilder, string? tenantId) where TEntity : class, IEntityTenant
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == tenantId);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _httpContextAccessor.HttpContext?.User?.FindFirst(Constants.CLAIM_TENANT)?.Value;

        foreach (var entry in ChangeTracker.Entries<IEntityTenant>())
        {
            if (entry.State == EntityState.Added && string.IsNullOrWhiteSpace(entry.Entity.TenantId))
            {
                entry.Entity.TenantId = tenantId ?? throw new InvalidOperationException("TenantId not found in user context.");
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}