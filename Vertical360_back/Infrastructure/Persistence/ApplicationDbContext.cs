using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Vertical360_back.Application.Interfaces.Services;
using Vertical360_back.Domain.Entityes;
using Vertical360_back.Domain.Entityes.Common;
using Vertical360_back.Domain.ValueObjects;

namespace Vertical360_back.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
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

    // DbSets
    public DbSet<Products> Products => Set<Products>();
    public DbSet<Companies> Companies => Set<Companies>();
    public DbSet<CompanyUserPermissions> CompanyUserPermissions => Set<CompanyUserPermissions>();
    public DbSet<LinkUserCompany> LinkUsersCommpany => Set<LinkUserCompany>();
}
