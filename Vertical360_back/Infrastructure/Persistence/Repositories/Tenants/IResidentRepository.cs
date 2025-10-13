using Vertical360_back.Application.Contracts.Resident;
using Vertical360_back.Domain.Entityes.Tenants;

namespace Vertical360_back.Infrastructure.Persistence.Repositories.Tenants
{
    public interface IResidentRepository
    {
        Task<IEnumerable<ResidentResponseDto>> GetAllAsync(string tenantId);
        Task<Resident?> GetByIdAsync(Guid id, string tenantId);
        Task AddAsync(Resident resident);
        Task UpdateAsync(Resident resident);
        Task DeleteAsync(Guid id, string tenantId);
        Task<bool> ExistsByDocumentAsync(string document, string tenantId);
        Task SaveChangesAsync();
    }
}
