using Microsoft.EntityFrameworkCore;
using Vertical360_backend.Application.DTOs.Companies;
using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Domain.Entities;
using Vertical360_backend.Infrastructure.Persistence;

namespace Vertical360_backend.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly MasterDbContext _context;

        public CompanyRepository(MasterDbContext context) => _context = context;

        public async Task CreateAsync(Companies company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CompanieResultDto>> GetAllAsync()
        {
            return await _context.Companies
                .AsNoTracking()
                .Include(c => c.Country)
                .Include(c => c.Department)
                .Include(c => c.City)
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => ToDto(c))
                .ToListAsync();
        }

        public async Task<CompanieResultDto?> GetByIdAsync(Guid id)
        {
            var c = await _context.Companies
                .AsNoTracking()
                .Include(c => c.Country)
                .Include(c => c.Department)
                .Include(c => c.City)
                .FirstOrDefaultAsync(c => c.Id == id);

            return c is null ? null : ToDto(c);
        }

        public async Task<Companies?> GetEntityByIdAsync(Guid id)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsByNitAsync(string nit, Guid? excludeId = null)
        {
            return await _context.Companies
                .AsNoTracking()
                .AnyAsync(c => c.Nit == nit && (excludeId == null || c.Id != excludeId));
        }

        public async Task UpdateAsync(Companies company)
        {
            company.UpdatedAt = DateTime.UtcNow;
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.Companies.FindAsync(id);
            if (entity is null) return false;

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        private static CompanieResultDto ToDto(Companies c) => new()
        {
            Id = c.Id,
            Nit = c.Nit,
            Name = c.Name,
            BusinessName = c.BusinessName,
            Phone = c.Phone,
            MobilePhone = c.MobilePhone,
            Address = c.Address,
            CountryId = c.CountryId,
            CountryName = c.Country?.Name,
            DepartmentId = c.DepartmentId,
            DepartmentName = c.Department?.Name,
            CityId = c.CityId,
            CityName = c.City?.Name,
            PostalCode = c.PostalCode,
            LegalRepresentativeName = c.LegalRepresentativeName,
            LegalRepresentativeEmail = c.LegalRepresentativeEmail,
            LegalRepresentativePhone = c.LegalRepresentativePhone,
            LegalRepresentativeMobile = c.LegalRepresentativeMobile,
            AdminName = c.AdminName,
            AdminEmail = c.AdminEmail,
            AdminPhone = c.AdminPhone,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };
    }
}

