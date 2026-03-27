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
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;
        private readonly ILogger<CompanyService> _logger;
        private readonly IConfiguration _configuration;

        public CompanyService(
            MasterDbContext context,
            UserManager<ApplicationUser> userManager,
            ITenantDatabaseService tenantDatabaseService,
            ICompanyRepository companyRepository,
            IHttpContextAccessor httpContextAccessor,
            IEmailService emailService,
            IEmailTemplateService templateService,
            ILogger<CompanyService> logger,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _tenantDatabaseService = tenantDatabaseService;
            _companyRepository = companyRepository;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _templateService = templateService;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<CompanieResultDto> CreateCompanyAsync(CompanyRequestDto dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User ?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("No se pudo obtener el usuario autenticado.");

            if (await _companyRepository.ExistsByNitAsync(dto.Nit))
                throw new InvalidOperationException($"Ya existe una unidad con el NIT '{dto.Nit}'.");

            var tenantKey = Guid.NewGuid().ToString("N");
            var adminPassword = string.IsNullOrWhiteSpace(dto.AdminPassword) ? GenerateTemporaryPassword() : dto.AdminPassword;
            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Name == dto.Country);
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Name == dto.Department);
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Name == dto.City);

            if (country == null)
                throw new InvalidOperationException($"No se encontró el país con ID '{dto.Country}'.");

            if (department == null)
                throw new InvalidOperationException($"No se encontró el departamento con ID '{dto.Department}'.");

            if (city == null)
                throw new InvalidOperationException($"No se encontró la ciudad con ID '{dto.City}'.");

            Companies? company = null;
            ApplicationUser? admin = null;
            var tenantDbCreated = false;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Crear la unidad residencial
                company = new Companies
                {
                    Nit = dto.Nit,
                    Name = dto.Name,
                    BusinessName = dto.BusinessName,
                    Phone = dto.Phone,
                    MobilePhone = dto.MobilePhone,
                    Address = dto.Address,
                    CountryId = country.Id,
                    DepartmentId = department.Id,
                    CityId = city.Id,
                    PostalCode = dto.PostalCode,
                    LegalRepresentativeName = dto.LegalRepresentativeName,
                    LegalRepresentativeEmail = dto.LegalRepresentativeEmail,
                    LegalRepresentativePhone = dto.LegalRepresentativePhone,
                    LegalRepresentativeMobile = dto.LegalRepresentativeMobile,
                    AdminName = dto.AdminName,
                    AdminEmail = dto.AdminEmail,
                    AdminPhone = dto.AdminPhone,
                    TenantKey = tenantKey,
                    CreatedByUserId = userId
                };
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                // Crear usuario administrador inicial
                admin = new ApplicationUser
                {
                    UserName = dto.AdminEmail,
                    Email = dto.AdminEmail,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(admin, adminPassword);
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(admin, RoleTypeEnum.Admin.ToString());

                // Vincular admin a la unidad
                _context.LinkUsersCompany.Add(new LinkUserCompany
                {
                    UserId = admin.Id,
                    CompanyId = company.Id,
                    RoleName = RoleTypeEnum.Admin.ToString()
                });
                await _context.SaveChangesAsync();

                // Crear BD del tenant (DDL fuera de transacción EF)
                await _tenantDatabaseService.CreateTenantDatabaseAsync(tenantKey);
                tenantDbCreated = true;

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Unidad '{Name}' (NIT: {Nit}) creada. TenantKey: {TenantKey}",
                    dto.Name, dto.Nit, tenantKey);

                // Enviar email de bienvenida (fuera de transacción — no crítico)
                await SendWelcomeEmailAsync(dto.AdminEmail, dto.AdminName, dto.Name, adminPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando unidad '{Name}'. Iniciando rollback.", dto.Name);

                await transaction.RollbackAsync();

                if (tenantDbCreated)
                {
                    try { await _tenantDatabaseService.DropTenantDatabaseAsync(tenantKey); }
                    catch (Exception dropEx)
                    {
                        _logger.LogError(dropEx, "Error en rollback de BD tenant. TenantKey: {TenantKey}", tenantKey);
                    }
                }

                if (admin != null)
                {
                    try { await _userManager.DeleteAsync(admin); }
                    catch (Exception deleteEx)
                    {
                        _logger.LogError(deleteEx, "Error en rollback de usuario admin. UserId: {UserId}", admin.Id);
                    }
                }

                throw new Exception($"Error al crear la unidad: {ex.Message}", ex);
            }

            return (await _companyRepository.GetByIdAsync(company.Id))!;
        }

        public async Task<List<CompanieResultDto>> GetAllAsync() => await _companyRepository.GetAllAsync();

        public async Task<CompanieResultDto> GetByIdAsync(Guid id)
        {
            var dto = await _companyRepository.GetByIdAsync(id);
            if (dto is null)
                throw new KeyNotFoundException($"Unidad con Id '{id}' no encontrada.");
            return dto;
        }

        public async Task<CompanieResultDto> UpdateAsync(Guid id, CompanyUpdateDto dto)
        {
            var entity = await _companyRepository.GetEntityByIdAsync(id) ?? throw new KeyNotFoundException($"Unidad con Id '{id}' no encontrada.");
            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Name == dto.Country);
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Name == dto.Department);
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Name == dto.City);

            if (country == null)
                throw new InvalidOperationException($"No se encontró el país con ID '{dto.Country}'.");

            if (department == null)
                throw new InvalidOperationException($"No se encontró el departamento con ID '{dto.Department}'.");

            if (city == null)
                throw new InvalidOperationException($"No se encontró la ciudad con ID '{dto.City}'.");

            if (!string.IsNullOrWhiteSpace(dto.Name)) entity.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.BusinessName)) entity.BusinessName = dto.BusinessName;
            if (dto.Phone is not null) entity.Phone = dto.Phone;
            if (dto.MobilePhone is not null) entity.MobilePhone = dto.MobilePhone;
            if (dto.Address is not null) entity.Address = dto.Address;
            if (dto.Country is not null) entity.CountryId = country.Id;
            if (dto.Country is not null) entity.DepartmentId = department.Id;
            if (dto.Country is not null) entity.CityId = city.Id;
            if (dto.PostalCode is not null) entity.PostalCode = dto.PostalCode;
            if (dto.LegalRepresentativeName is not null) entity.LegalRepresentativeName = dto.LegalRepresentativeName;
            if (dto.LegalRepresentativeEmail is not null) entity.LegalRepresentativeEmail = dto.LegalRepresentativeEmail;
            if (dto.LegalRepresentativePhone is not null) entity.LegalRepresentativePhone = dto.LegalRepresentativePhone;
            if (dto.LegalRepresentativeMobile is not null) entity.LegalRepresentativeMobile = dto.LegalRepresentativeMobile;
            if (!string.IsNullOrWhiteSpace(dto.AdminName)) entity.AdminName = dto.AdminName;
            if (!string.IsNullOrWhiteSpace(dto.AdminEmail)) entity.AdminEmail = dto.AdminEmail;
            if (dto.AdminPhone is not null) entity.AdminPhone = dto.AdminPhone;

            await _companyRepository.UpdateAsync(entity);

            return (await _companyRepository.GetByIdAsync(id))!;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var exists = await _companyRepository.GetEntityByIdAsync(id);
            if (exists is null)
                throw new KeyNotFoundException($"Unidad con Id '{id}' no encontrada.");

            return await _companyRepository.DeleteAsync(id);
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static string GenerateTemporaryPassword()
        {
            const string lower = "abcdefghijkmnopqrstuvwxyz";
            const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string digits = "23456789";
            const string special = "!@#$%&*";

            var rng = new Random();
            var pwd = new char[12];
            pwd[0] = upper[rng.Next(upper.Length)];
            pwd[1] = lower[rng.Next(lower.Length)];
            pwd[2] = digits[rng.Next(digits.Length)];
            pwd[3] = special[rng.Next(special.Length)];
            var all = lower + upper + digits + special;
            for (int i = 4; i < 12; i++)
                pwd[i] = all[rng.Next(all.Length)];

            return new string(pwd.OrderBy(_ => Guid.NewGuid()).ToArray());
        }

        private async Task SendWelcomeEmailAsync(string adminEmail, string adminName, string companyName, string password)
        {
            try
            {
                var loginUrl = _configuration["App:LoginUrl"] ?? "http://localhost:5173/login";
                var body = await _templateService.RenderAsync("WelcomeAdmin", new Dictionary<string, string>
                {
                    { "ADMIN_NAME", adminName },
                    { "COMPANY_NAME", companyName },
                    { "ADMIN_EMAIL", adminEmail },
                    { "ADMIN_PASSWORD", password },
                    { "LOGIN_URL", loginUrl }
                });

                await _emailService.SendAsync(
                    to: adminEmail,
                    subject: $"Bienvenido a Vertical360 — {companyName}",
                    body: body);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo enviar el email de bienvenida a {Email}", adminEmail);
            }
        }
    }
}

