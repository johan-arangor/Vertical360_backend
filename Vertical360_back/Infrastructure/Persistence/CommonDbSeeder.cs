using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Vertical360_back.Application.Contracts.DTOs;
using Vertical360_back.Application.Interfaces.Services;
using Vertical360_back.Domain.Entityes;
using Vertical360_back.Domain.Entityes.Common;

namespace Vertical360_back.Infrastructure.Persistence
{
    public class CommonDbSeeder : ICommonDbSeeder
    {
        private readonly CommonDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CommonDbSeeder(CommonDbContext commonDbSeeder, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = commonDbSeeder;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            // Asegurar que la Base de Datos esté actualizada (se pueden usar migraciones)
            await _context.Database.MigrateAsync();
            await SeedIdentityAsync();
            await SeedInitialCompanyAndLink();

            if (!await _context.Countries.AnyAsync()) await SeedCountriesAsync();
            if (!await _context.Departments.AnyAsync()) await SeedDepartmentsAsync();
            if (!await _context.Cities.AnyAsync()) await SeedCitiesAsync();
        }
        private async Task SeedIdentityAsync()
        {
            await SeedRolesAsync();
            await SeedSuperAdminUserAsync();
        }

        private async Task SeedRolesAsync()
        {
            const string superAdminRole = "PlatformSuperAdmin";

            if (!await _roleManager.RoleExistsAsync(superAdminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(superAdminRole));
            }
        }

        private async Task SeedSuperAdminUserAsync()
        {
            const string adminEmail = "admin@vertical360.com";
            const string adminPassword = "SuperAdminPass123!";
            const string superAdminRole = "PlatformSuperAdmin";

            var user = await _userManager.FindByEmailAsync(adminEmail);

            if (user == null)
            {
                var superUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(superUser, adminPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(superUser, superAdminRole);
                }
            }
        }

        public async Task SeedInitialCompanyAndLink()
        {
            const string adminEmail = "admin@vertical360.com";
            var user = await _userManager.FindByEmailAsync(adminEmail);

            if (user == null) return;

            const string companyName = "Vertical 360 - Initial Tenant";
            const string tenantDbConnection = "Server=localhost;Database=tenant_vertical360;User Id=root;Password=root;";

            // Verifica si ya existe una compañía
            if (!_context.Companies.Any(c => c.Name == companyName))
            {
                var initialCompany = new Companies
                {
                    Id = Guid.NewGuid(),
                    Name = companyName,
                    // Usamos una cadena de conexión ficticia para el primer tenant
                    ConnectionString = tenantDbConnection,
                    UserCreationId = user.Id
                };

                _context.Companies.Add(initialCompany);
                await _context.SaveChangesAsync();

                // Crear el enlace LinkUserCompany (asumo que LinkUserCompany existe)
                var link = new LinkUserCompany
                {
                    CompanyId = initialCompany.Id,
                    UserId = user.Id
                };

                _context.LinkUsersCommpany.Add(link);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedCountriesAsync()
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "SeedData", "countries.json");

            if (!File.Exists(filePath)) return;

            var json = await File.ReadAllTextAsync(filePath);
            // Usa CountrySeedDto
            var countryDtos = JsonSerializer.Deserialize<List<CountrySeedDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (countryDtos != null)
            {
                var countriesToSeed = new List<Countries>();
                foreach (var dto in countryDtos)
                {
                    countriesToSeed.Add(new Countries
                    {
                        Id = Guid.NewGuid(),
                        Name = dto.Name,
                        Code = dto.Code,
                        PhoneCode = dto.PhoneCode
                    });
                }
                await _context.Countries.AddRangeAsync(countriesToSeed);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedDepartmentsAsync()
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "SeedData", "departments.json");

            if (!File.Exists(filePath)) return;

            // Cargar el JSON en un DTO temporal (para mantener el CountryCode)
            var json = await File.ReadAllTextAsync(filePath);
            var departmentDtos = JsonSerializer.Deserialize<List<DepartmentSeedDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (departmentDtos == null || !departmentDtos.Any()) return;

            // Obtener el mapeo de Países (Code -> Id) desde la BD
            var countriesMap = await _context.Countries
                .ToDictionaryAsync(c => c.Code, c => c.Id);

            var departmentsToSeed = new List<Departments>();

            // Transformar DTOs a Entidades, resolviendo el CountryId
            foreach (var dto in departmentDtos)
            {
                if (countriesMap.TryGetValue(dto.CountryCode, out Guid countryId))
                {
                    departmentsToSeed.Add(new Departments
                    {
                        Id = Guid.NewGuid(),
                        Name = dto.Name,
                        Code = dto.Code,
                        CountryId = countryId
                    });
                }
            }

            if (departmentsToSeed.Any())
            {
                await _context.Departments.AddRangeAsync(departmentsToSeed);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedCitiesAsync()
        {
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "SeedData", "cities.json");

            if (!File.Exists(filePath)) return;

            // Cargar el JSON en un DTO temporal (para mantener el DepartmentCode)
            var json = await File.ReadAllTextAsync(filePath);
            var cityDtos = JsonSerializer.Deserialize<List<CitySeedDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (cityDtos == null || !cityDtos.Any()) return;

            // Obtener el mapeo de Departamentos (Code -> Id) desde la BD
            var departmentsMap = await _context.Departments
                .ToDictionaryAsync(d => d.Code, d => d.Id);

            var citiesToSeed = new List<Cities>();

            // Transformar DTOs a Entidades, resolviendo el DepartmentId
            foreach (var dto in cityDtos)
            {
                if (departmentsMap.TryGetValue(dto.DepartmentCode, out Guid departmentId))
                {
                    citiesToSeed.Add(new Cities
                    {
                        Id = Guid.NewGuid(),
                        Name = dto.Name,
                        Code = dto.Code,
                        DepartmentId = departmentId
                    });
                }
            }

            if (citiesToSeed.Any())
            {
                await _context.Cities.AddRangeAsync(citiesToSeed);
                await _context.SaveChangesAsync();
            }
        }
    }
}
