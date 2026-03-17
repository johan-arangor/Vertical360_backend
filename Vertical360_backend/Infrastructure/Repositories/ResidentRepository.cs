using Microsoft.EntityFrameworkCore;
using Vertical360_backend.Application.DTOs.Residents;
using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Domain.Entities;

namespace Vertical360_backend.Infrastructure.Repositories
{
    public class ResidentRepository : IResidentRepository
    {
        private readonly ITenantDbContextFactory _contextFactory;

        public ResidentRepository(ITenantDbContextFactory contextFactory)
            => _contextFactory = contextFactory;

        public async Task AddAsync(Resident resident)
        {
            await using var ctx = _contextFactory.CreateContext();
            await ctx.Residents.AddAsync(resident);
            await ctx.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            // No-op: cada operación abre y cierra su propio contexto.
            // Se mantiene para compatibilidad con la interfaz.
        }

        public async Task<IEnumerable<ResidentResultDto>> GetAllAsync(string tenantId)
        {
            await using var ctx = _contextFactory.CreateContext();
            return await ctx.Residents
                .AsNoTracking()
                .Where(r => r.TenantId == tenantId)
                .Select(r => new ResidentResultDto
                {
                    Id = r.Id,
                    FullName = r.FullName,
                    Document = r.Document,
                    Email = r.Email,
                    PhoneNumber = r.PhoneNumber,
                    IsOwner = r.IsOwner
                })
                .ToListAsync();
        }

        public async Task<Resident?> GetByIdAsync(Guid id, string tenantId)
        {
            await using var ctx = _contextFactory.CreateContext();
            return await ctx.Residents
                .FirstOrDefaultAsync(r => r.Id == id && r.TenantId == tenantId);
        }

        public async Task UpdateAsync(Resident resident)
        {
            await using var ctx = _contextFactory.CreateContext();
            ctx.Residents.Update(resident);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await using var ctx = _contextFactory.CreateContext();
            var entity = await ctx.Residents.FindAsync(id);
            if (entity != null)
            {
                entity.IsActive = false;
                await ctx.SaveChangesAsync();
            }
        }
    }
}
