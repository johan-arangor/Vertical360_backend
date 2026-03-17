using Vertical360_backend.Application.DTOs.Companies;
using Vertical360_backend.Domain.Entities;

namespace Vertical360_backend.Application.Interfaces
{
    public interface ICompanyRepository
    {
        Task CreateAsync(Companies company);
        Task<List<CompanieResultDto>> GetAllAsync();
        Task<CompanieResultDto?> GetByIdAsync(Guid id);
        Task<Companies?> GetEntityByIdAsync(Guid id);
        Task<bool> ExistsByNitAsync(string nit, Guid? excludeId = null);
        Task UpdateAsync(Companies company);
        Task<bool> DeleteAsync(Guid id);
    }
}

