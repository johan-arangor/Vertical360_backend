using Microsoft.AspNetCore.Identity;

namespace Vertical360_back.Infrastructure.Persistence.Seed
{
    public class IdentityDataSeeder
    {
        public static async Task SeedSuperAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            const string roleName = "SuperAdmin";
            const string email = "johan.arangor@gmail.com";
            const string password = "123456";

            // 1. Crear el rol si no existe
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole(roleName);
                await roleManager.CreateAsync(role);
            }

            // 2. Crear el usuario si no existe
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    throw new Exception($"Error creando el usuario inicial: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // 3. Asignar el rol al usuario si aún no lo tiene
            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}
