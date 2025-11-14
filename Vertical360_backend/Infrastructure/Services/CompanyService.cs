using Microsoft.AspNetCore.Identity;
using Vertical360_backend.Application.DTOs.Companies;
using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Domain.Entities;
using Vertical360_backend.Domain.Enums;
using Vertical360_backend.Infrastructure.Persistence;

namespace Vertical360_backend.Infrastructure.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITenantDatabaseService _tenantService;
        private readonly ICompanyRepository _companyRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CompanyService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ITenantDatabaseService tenantService, ICompanyRepository companyRepository, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _tenantService = tenantService;
            _companyRepository = companyRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task CreateCompanyAsync(CompanyRequestDto dto)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
                            ?? _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    throw new Exception("No se pudo obtener el usuario autenticado.");

                // Crear la compañía
                var company = new Companies
                {
                    Name = dto.Name,
                    TenantKey = Guid.NewGuid().ToString(),
                    CreatedByUserId = userId
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                // Crear usuario admin
                var admin = new ApplicationUser
                {
                    UserName = dto.AdminEmail,
                    Email = dto.AdminEmail,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(admin, dto.AdminPassword);
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(admin, RoleTypeEnum.Admin.ToString());

                // Vincular usuario con compañía
                _context.LinkUsersCompany.Add(new LinkUserCompany
                {
                    UserId = admin.Id,
                    CompanyId = company.Id,
                    RoleName = RoleTypeEnum.Admin.ToString()
                });

                await _context.SaveChangesAsync();

                // Crear base de datos del tenant desde la semilla
                await _tenantService.CreateTenantDatabaseAsync(company.TenantKey);
            } catch (Exception ex)
            {
                throw new Exception("Error creating company: " + ex.Message);
            }
        }

        public async Task<List<CompanieResultDto>> GetAllAsync()
        {
            return await _companyRepository.GetAllAsync();
        }
    }
}
