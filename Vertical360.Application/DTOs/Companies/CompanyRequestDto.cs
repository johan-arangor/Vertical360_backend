namespace Vertical360.Application.DTOs.Companies
{
    public class CompanyRequestDto
    {
        public string Name { get; set; } = string.Empty!;
        public string AdminEmail { get; set; } = string.Empty!;
        public string AdminPassword { get; set; } = string.Empty!;
    }
}
