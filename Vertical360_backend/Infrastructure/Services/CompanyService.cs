using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vertical360_backend.Application.DTOs.Companies;
using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Domain.Entities;
using Vertical360_backend.Domain.Enums;
using Vertical360_backend.Infrastructure.Persistence;

namespace Vertical360_backend.Infrastructure.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly MasterDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITenantDatabaseService _tenantDatabaseService;
        private readonly ICompanyRepository _companyRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(
            MasterDbContext context,
            UserManager<ApplicationUser> userManager,
            ITenantDatabaseService tenantDatabaseService,
            ICompanyRepository companyRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CompanyService> logger)
        {
            _context = context;
            _userManager = userManager;
            _tenantDatabaseService = tenantDatabaseService;
            _companyRepository = companyRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task CreateCompanyAsync(CompanyRequestDto dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User
                ?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new Exception("No se pudo obtener el usuario autenticado.");

            var tenantKey = Guid.NewGuid().ToString("N"); // sin guiones para nombres de BD
            Companies? company = null;
            ApplicationUser? admin = null;
            var tenantDbCreated = false;

            // Transacción en master DB para company + user + link
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Crear la compañía
                company = new Companies
                {
                    Name = dto.Name,
                    TenantKey = tenantKey,
                    CreatedByUserId = userId
                };
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                // 2. Crear usuario admin inicial
                admin = new ApplicationUser
                {
                    UserName = dto.AdminEmail,
                    Email = dto.AdminEmail,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(admin, dto.AdminPassword);
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(admin, RoleTypeEnum.Admin.ToString());

                // 3. Vincular admin a la compañía
                _context.LinkUsersCompany.Add(new LinkUserCompany
                {
                    UserId = admin.Id,
                    CompanyId = company.Id,
                    RoleName = RoleTypeEnum.Admin.ToString()
                });
                await _context.SaveChangesAsync();

                // 4. Crear la BD del tenant (fuera de la transacción de master — operación DDL)
                await _tenantDatabaseService.CreateTenantDatabaseAsync(tenantKey);
                tenantDbCreated = true;

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Compañía '{Name}' creada exitosamente. TenantKey: {TenantKey}",
                    dto.Name, tenantKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando compañía '{Name}'. Iniciando rollback.", dto.Name);

                await transaction.RollbackAsync();

                // Rollback de la BD del tenant si fue creada
                if (tenantDbCreated)
                {
                    try
                    {
                        await _tenantDatabaseService.DropTenantDatabaseAsync(tenantKey);
                    }
                    catch (Exception dropEx)
                    {
                        _logger.LogError(dropEx,
                            "Error al eliminar la BD del tenant durante rollback. TenantKey: {TenantKey}",
                            tenantKey);
                    }
                }

                // Rollback del usuario de Identity (no está en la transacción EF)
                if (admin != null)
                {
                    try
                    {
                        await _userManager.DeleteAsync(admin);
                    }
                    catch (Exception deleteEx)
                    {
                        _logger.LogError(deleteEx,
                            "Error al eliminar el usuario admin durante rollback. UserId: {UserId}",
                            admin.Id);
                    }
                }

                throw new Exception($"Error al crear la compañía: {ex.Message}", ex);
            }
        }

        public async Task<List<CompanieResultDto>> GetAllAsync()
        {
            return await _companyRepository.GetAllAsync();
        }
    }
}
