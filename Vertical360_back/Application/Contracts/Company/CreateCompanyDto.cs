namespace Vertical360_back.Application.Contracts.Company
{
    public class CreateCompanyDto
    {
        public string CompanyName { get; set; } = null!;
        public string AdminUserName { get; set; } = null!;
        public string AdminEmail { get; set; } = null!;
        public string AdminPassword { get; set; } = null!;
    }
}
