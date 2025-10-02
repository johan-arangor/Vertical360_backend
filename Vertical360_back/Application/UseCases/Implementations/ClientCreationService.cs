using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vertical360_back.Application.Interfaces.Services;
using Vertical360_back.Infrastructure.Persistence;

namespace Vertical360_back.Application.UseCases.Implementations
{
    public class ClientCreationService : IClientCreationService
    {
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public ClientCreationService(IConfiguration configuration, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task CreateClientAndSeedAsync(string clientName, string adminEmail, string adminPassword)
        {
            // Estandarizar el nombre de la BD.
            var dbName = $"tenant_{clientName.ToLowerInvariant().Replace(" ", "_").Replace(".", "_").Replace("-","_")}";
            var template = _configuration.GetConnectionString("TenantConnectionTemplate");
            var connectionString = string.Format(template, dbName);

            // CONTEXTO DEL CLIENTE (MANUAL)
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            using (var clientContext = new ApplicationDbContext(optionsBuilder.Options))
            {
                // APLICAR MIGRACIONES A LA NUEVA BD
                await clientContext.Database.MigrateAsync();
            }

            // CREAR ROL Y USUARIO ADMIN (Se requiere la instancia de UserManager/RoleManager
            // que ya fue registrada en Program.cs y usa ApplicationDbContext)
            const string roleName = "SuperAdmin";
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Crear Usuario SuperAdmin
            var superAdminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(superAdminUser, adminPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(superAdminUser, roleName);

                // LÓGICA PARA ASIGNAR EL CLIENTE/TENANT AL USUARIO (EN LA BD COMÚN)
                // Esto requiere inyectar el CommonDbContext y crear la entrada LinkUserCompany
            }
        }
    }
}
