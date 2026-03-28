using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json;
using Vertical360.Core.Entities;

namespace Vertical360.Infrastructure.Persistence
{
    public class ApplicationDbSeeder
    {
        private readonly MasterDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ApplicationDbSeeder(
            MasterDbContext context,
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
                // Asegura que la base de datos compartida esté creada
                await _context.Database.EnsureCreatedAsync();

                // Roles iniciales
                var roles = new[] { "PlatformSuperAdmin", "ADMIN", "RESIDENT", "OWNER", "SECURITY", "ASSISTANT" };
                foreach (var r in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(r))
                        await _roleManager.CreateAsync(new ApplicationRole { Name = r });
                }

                // SuperAdmin global
                var superEmail = "superadmin@vertical360.com";
                var superUser = await _userManager.FindByEmailAsync(superEmail);
                if (superUser == null)
                {
                    superUser = new ApplicationUser
                    {
                        UserName = "1234567890",
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

                // Crear una compañía inicial
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

                    // ?? Vincular el SuperAdmin a la compañía
                    _context.LinkUsersCompany.Add(new LinkUserCompany
                    {
                        CompanyId = company.Id,
                        UserId = superUser.Id,
                        RoleName = "PlatformSuperAdmin"
                    });
                    await _context.SaveChangesAsync();
                }

                // ---------------------------
                // Seed geográfico desde JSON
                // ---------------------------
                var seedDir = Path.Combine(AppContext.BaseDirectory, "SeedData");

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Countries
                var countriesFile = Path.Combine(seedDir, "countries.json");
                if (File.Exists(countriesFile))
                {
                    var text = await File.ReadAllTextAsync(countriesFile);
                    var countries = JsonSerializer.Deserialize<List<CountrySeedModel>>(text, jsonOptions) ?? new List<CountrySeedModel>();
                    foreach (var c in countries)
                    {
                        if (string.IsNullOrWhiteSpace(c.Name)) continue;

                        if (!await _context.Countries.AnyAsync(x => x.Name == c.Name))
                        {
                            _context.Countries.Add(new Country
                            {
                                Name = c.Name,
                                Acronym = c.Acronym,
                                Code = c.Code
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Departments
                var departmentsFile = Path.Combine(seedDir, "departments.json");
                if (File.Exists(departmentsFile))
                {
                    var text = await File.ReadAllTextAsync(departmentsFile);
                    var departments = JsonSerializer.Deserialize<List<DepartmentSeedModel>>(text, jsonOptions) ?? new List<DepartmentSeedModel>();
                    foreach (var d in departments)
                    {
                        if (string.IsNullOrWhiteSpace(d.Name) || string.IsNullOrWhiteSpace(d.Country)) continue;

                        var country = await _context.Countries.FirstOrDefaultAsync(c => c.Name == d.Country);
                        if (country == null) continue;

                        if (!await _context.Departments.AnyAsync(x => x.Name == d.Name && x.CountryId == country.Id))
                        {
                            _context.Departments.Add(new Department
                            {
                                Name = d.Name,
                                Acronym = d.Acronym,
                                Code = d.Code,
                                CountryId = country.Id,
                                Country = country
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Cities
                var citiesFile = Path.Combine(seedDir, "cities.json");
                if (File.Exists(citiesFile))
                {
                    var text = await File.ReadAllTextAsync(citiesFile);
                    var cities = JsonSerializer.Deserialize<List<CitySeedModel>>(text, jsonOptions) ?? new List<CitySeedModel>();
                    foreach (var ct in cities)
                    {
                        if (string.IsNullOrWhiteSpace(ct.Name) || string.IsNullOrWhiteSpace(ct.Department)) continue;

                        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Name == ct.Department);
                        if (department == null) continue;

                        if (!await _context.Cities.AnyAsync(x => x.Name == ct.Name && x.DepartmentId == department.Id))
                        {
                            _context.Cities.Add(new City
                            {
                                Name = ct.Name,
                                Acronym = ct.Acronym,
                                Code = ct.Code,
                                DepartmentId = department.Id,
                                Department = department
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en ApplicationDbSeeder.SeedAsync: {ex.Message}", ex);
            }
        }

        // Modelos para deserializar JSON de seed
        private class CountrySeedModel
        {
            public string Name { get; set; } = null!;
            public string? Acronym { get; set; }
            public string? Code { get; set; }
        }

        private class DepartmentSeedModel
        {
            public string Name { get; set; } = null!;
            public string Country { get; set; } = null!;
            public string? Acronym { get; set; }
            public string? Code { get; set; }
        }

        private class CitySeedModel
        {
            public string Name { get; set; } = null!;
            public string Department { get; set; } = null!;
            public string? Acronym { get; set; }
            public string? Code { get; set; }
        }
    }
}