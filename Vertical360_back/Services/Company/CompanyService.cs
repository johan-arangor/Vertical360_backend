using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Security.Claims;
using Vertical360_back.Application.Common.Exceptions;
using Vertical360_back.Application.Contracts.Company;
using Vertical360_back.Domain.Entityes.Common;
using Vertical360_back.Infrastructure.Persistence.Repositories;

namespace Vertical360_back.Services.Company
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<CompanyService> _logger;

        private readonly string[] defaultRoles = new[]
        {
            "ADMIN", "SECURITY", "ASSISTANT", "SUPERVISOR", "RESIDENT", "OWNER"
        };

        public CompanyService(ICompanyRepository repository, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<CompanyService> logger)
        {
            _repository = repository;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto model, Guid userId)
        {
            try
            {
                await ValidateCompanyDoesNotExist(model.CompanyName);

                await EnsureDefaultRolesExist();

                var adminUser = await EnsureAdminUserExists(model);

                var company = await CreateCompany(model.CompanyName, userId);

                await CreateLinkRelations(company.Id, adminUser.Id, userId);

                _logger.LogInformation("Compañía creada exitosamente: {CompanyName}", company.Name);

                return new CompanyDto
                {
                    Id = company.Id,
                    Name = company.Name.ToUpper()
                };
            }
            catch (AppException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error de concurrencia al crear compañía.");
                throw new CompanyCreationException("Error de concurrencia al guardar la compañía.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear la compañía.");
                throw new CompanyCreationException(ex.Message);
            }
        }

        public async Task<List<CompanyDto>> GetAllCompaniesAsync()
            => (await _repository.GetAllAsync())
                .Select(c => new CompanyDto { Id = c.Id, Name = c.Name })
                .ToList();

        public async Task<CompanyDto?> GetCompanyByIdAsync(Guid id)
        {
            var company = await _repository.GetByIdAsync(id);

            if (company == null) return null;

            return new CompanyDto { Id = company.Id, Name = company.Name };
        }

        public async Task UpdateCompanyAsync(Guid id, string name)
        {
            var company = await _repository.GetByIdAsync(id);

            if (company == null) throw new Exception("Company not found");

            company.Name = name;
            await _repository.UpdateAsync(company);
        }

        public async Task DeleteCompanyAsync(Guid id)
            => await _repository.DeleteAsync(id);

        private async Task ValidateCompanyDoesNotExist(string companyName)
        {
            var existingCompany = await _repository.FirstOrDefaultAsync(c => c.Name.ToUpper() == companyName.ToUpper());
            if (existingCompany != null)
                throw new AppException("COMPANY_DUPLICATE", $"La compañía '{companyName}' ya existe.");
        }

        private async Task EnsureDefaultRolesExist()
        {
            foreach (var role in defaultRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                    _logger.LogInformation("Rol creado: {Role}", role);
                }
            }
        }

        private async Task<IdentityUser> EnsureAdminUserExists(CreateCompanyDto model)
        {
            var adminUser = await _userManager.FindByEmailAsync(model.AdminUserName.ToLower());

            if (adminUser != null)
            {
                _logger.LogInformation("Usuario administrador existente reutilizado: {Documento}", model.AdminUserName);
                return adminUser;
            }

            // Crear nuevo usuario admin
            adminUser = new IdentityUser
            {
                UserName = model.AdminUserName,
                Email = model.AdminEmail.ToLower(),
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(adminUser, model.AdminPassword);

            if (!result.Succeeded)
            {
                var details = string.Join("; ", result.Errors.Select(e => e.Description));

                if (result.Errors.Any(e => e.Code.Contains("Password", StringComparison.OrdinalIgnoreCase)))
                    throw new WeakPasswordException(details);

                throw new UserCreationException(details);
            }

            await _userManager.AddToRoleAsync(adminUser, "ADMIN");
            _logger.LogInformation("Usuario administrador creado: {Documento}", model.AdminUserName);

            return adminUser;
        }

        private async Task<Companies> CreateCompany(string companyName, Guid userId)
        {
            var company = new Companies
            {
                Id = Guid.NewGuid(),
                Name = companyName.ToUpper(),
                UserCreationId = userId.ToString(),
                linkUserCompanies = new List<LinkUserCompany>()
            };

            await _repository.AddAsync(company);
            return company;
        }

        private async Task CreateLinkRelations(Guid companyId, string adminUserId, Guid creatorUserId)
        {
            var linkRelations = new List<LinkUserCompany>
            {
                // Usuario creador (SuperAdmin)
                new LinkUserCompany
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companyId,
                    UserId = creatorUserId.ToString(),
                    DateTimeCreate = DateTime.UtcNow,
                    statusLink = Domain.Enums.StatusLinkEnum.Accepted
                },
                // Usuario administrador
                new LinkUserCompany
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companyId,
                    UserId = adminUserId,
                    DateTimeCreate = DateTime.UtcNow,
                    statusLink = Domain.Enums.StatusLinkEnum.Accepted
                }
            };

            await _repository.AddLinksAsync(linkRelations);
        }
    }
}
