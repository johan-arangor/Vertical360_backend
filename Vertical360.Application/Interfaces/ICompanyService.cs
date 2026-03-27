using Vertical360.Application.DTOs.Companies;

namespace Vertical360.Application.Interfaces
{
    public interface ICompanyService
    {
        Task CreateCompanyAsync(CompanyRequestDto dto);
        Task<List<CompanieResultDto>> GetAllAsync();
    }
}
