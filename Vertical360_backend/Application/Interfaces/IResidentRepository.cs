using Vertical360_backend.Application.DTOs.Residents;
using Vertical360_backend.Domain.Entities;

namespace Vertical360_backend.Application.Interfaces
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
