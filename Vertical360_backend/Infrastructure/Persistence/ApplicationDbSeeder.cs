using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vertical360_backend.Domain.Entities;

namespace Vertical360_backend.Infrastructure.Persistence
{
    public class ApplicationDbSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ApplicationDbSeeder(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            try
            {
                // 🧱 Asegura que la base de datos compartida esté creada
                await _context.Database.EnsureCreatedAsync();

                // 👥 Roles iniciales
                var roles = new[] { "PlatformSuperAdmin", "ADMIN", "RESIDENT", "OWNER", "SECURITY", "ASSISTANT" };
                foreach (var r in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(r))
                        await _roleManager.CreateAsync(new ApplicationRole { Name = r });
                }

                // 👤 SuperAdmin global
                var superEmail = "superadmin@vertical360.local";
                var superUser = await _userManager.FindByEmailAsync(superEmail);
                if (superUser == null)
                {
                    superUser = new ApplicationUser
                    {
                        UserName = "superadmin",
                        Email = superEmail,
                        EmailConfirmed = true
                    };
                    var result = await _userManager.CreateAsync(superUser, "SuperAdminPass123!");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(superUser, "PlatformSuperAdmin");
                    }
                    else
                    {
                        throw new Exception("Error creando usuario SuperAdmin: " +
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }

                // 🏢 Crear una compañía inicial
                if (!await _context.Companies.AnyAsync())
                {
                    var company = new Companies
                    {
                        Name = "Initial Tenant",
                        TenantKey = Guid.NewGuid().ToString(),
                        CreatedByUserId = superUser.Id
                    };

                    _context.Companies.Add(company);
                    await _context.SaveChangesAsync();

                    // 🔗 Vincular el SuperAdmin a la compañía
                    _context.LinkUsersCompany.Add(new LinkUserCompany
                    {
                        CompanyId = company.Id,
                        UserId = superUser.Id,
                        RoleName = "PlatformSuperAdmin"
                    });
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en ApplicationDbSeeder.SeedAsync: {ex.Message}", ex);
            }
        }
    }
}