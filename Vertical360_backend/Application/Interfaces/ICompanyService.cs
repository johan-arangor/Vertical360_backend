using Vertical360_backend.Application.DTOs.Companies;

namespace Vertical360_backend.Application.Interfaces
{
    public interface ICompanyService
    {
        Task CreateCompanyAsync(CompanyRequestDto dto);
        Task<List<CompanieResultDto>> GetAllAsync();
    }
}
