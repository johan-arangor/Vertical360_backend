using Vertical360_back.Application.Contracts.Company;

namespace Vertical360_back.Services.Company
{
    public interface ICompanyService
    {
        Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto dto, Guid UserId);
        Task<List<CompanyDto>> GetAllCompaniesAsync();
        Task<CompanyDto?> GetCompanyByIdAsync(Guid id);
        Task UpdateCompanyAsync(Guid id, string name);
        Task DeleteCompanyAsync(Guid id);
    }
}
