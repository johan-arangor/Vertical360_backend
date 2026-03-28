using Vertical360.Application.DTOs.Residents;
using Vertical360.Core.Entities;

namespace Vertical360.Application.Interfaces
{
    public interface IResidentRepository
    {
        Task AddAsync(Resident resident);
        Task SaveChangesAsync();
        Task<IEnumerable<ResidentResultDto>> GetAllAsync(string tenantId);
        Task<Resident?> GetByIdAsync(Guid id, string tenantId);
        Task UpdateAsync(Resident residentResult);
        Task DeleteAsync(Guid id);
    }
}
