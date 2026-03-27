using Vertical360.Application.DTOs.Companies;
using Vertical360.Core.Entities;

namespace Vertical360.Application.Interfaces
{
    public interface ICompanyRepository
    {
        Task CreateAsync(Companies company);
        Task<List<CompanieResultDto>> GetAllAsync();
        Task<CompanieResultDto?> GetByIdAsync(Guid id);
    }
}
