using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Vertical360_back.Application.Contracts.Resident;
using Vertical360_back.Domain.Entityes.Tenants;

namespace Vertical360_back.Infrastructure.Persistence.Repositories.Tenants
{
    public class ResidentRepository : IResidentRepository
    {
        private readonly ApplicationDbContext _context;

        public ResidentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResidentResponseDto>> GetAllAsync(string tenantId)
        {
            var residents = await _context.Residents
                .IgnoreQueryFilters()
                .Where(r => r.TenantId == tenantId.Trim() && r.IsActive)
                .Select(r => new ResidentResponseDto
                {
                    Id = r.Id,
                    FullName = r.FullName,
                    DocumentType = r.DocumentType,
                    Document = r.Document,
                    IsOwner = r.IsOwner,
                    DateOfBirth = r.AdditionalInfo!.DateOfBirth,
                    GenderType = r.AdditionalInfo!.GenderType
                })
                .ToListAsync();

            return residents;
        }

        public async Task<Resident?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _context.Residents
                .Include(r => r.AdditionalInfo)
                .FirstOrDefaultAsync(r => r.Id == id && r.TenantId == tenantId);
        }

        public async Task AddAsync(Resident resident)
        {
            await _context.Residents.AddAsync(resident);
        }

        public async Task UpdateAsync(Resident resident)
        {
            _context.Residents.Update(resident);
        }

        public async Task DeleteAsync(Guid id, string tenantId)
        {
            var resident = await GetByIdAsync(id, tenantId);
            if (resident != null)
            {
                _context.Residents.Remove(resident);
            }
        }

        public async Task<bool> ExistsByDocumentAsync(string document, string tenantId)
        {
            return await _context.Residents
                .AnyAsync(r => r.Document == document && r.TenantId == tenantId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
