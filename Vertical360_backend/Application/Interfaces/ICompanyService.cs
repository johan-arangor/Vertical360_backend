using Vertical360_backend.Application.DTOs.Companies;

namespace Vertical360_backend.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanieResultDto> CreateCompanyAsync(CompanyRequestDto dto);
        Task<List<CompanieResultDto>> GetAllAsync();
        Task<CompanieResultDto> GetByIdAsync(Guid id);
        Task<CompanieResultDto> UpdateAsync(Guid id, CompanyUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}

