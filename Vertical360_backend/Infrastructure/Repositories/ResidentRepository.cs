using Microsoft.EntityFrameworkCore;
using Vertical360_backend.Application.DTOs.Residents;
using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Domain.Entities;
using Vertical360_backend.Infrastructure.Persistence;

namespace Vertical360_backend.Infrastructure.Repositories
{
    public class ResidentRepository : IResidentRepository
    {
        private readonly ApplicationDbContext _context;
        public ResidentRepository(ApplicationDbContext context) => _context = context;

        public async Task AddAsync(Resident resident) => await _context.Residents.AddAsync(resident);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<IEnumerable<ResidentResultDto>> GetAllAsync(string tenantId)
        {
            var listResult = await _context.Residents.Where(r => r.TenantId == tenantId).ToListAsync();
        
            var result = listResult.Select(r => new ResidentResultDto
            {
                Id = r.Id,
                FullName = r.FullName,
                Document = r.Document,
                Email = r.Email,
                PhoneNumber = r.PhoneNumber,
                IsOwner = r.IsOwner
            });

            return result;
        }

        public async Task<Resident?> GetByIdAsync(Guid id, string tenantId)
        { 
            var result = await _context.Residents.FirstOrDefaultAsync(r => r.Id == id && r.TenantId == tenantId);

            if (result == null) return null;

            return result;
        }

        public async Task UpdateAsync(Resident resident)
        {
            _context.Residents.Update(resident);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Residents.FindAsync(id);

            if (entity != null)
            {
                entity.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}
