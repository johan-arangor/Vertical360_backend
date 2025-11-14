namespace Vertical360_backend.Application.DTOs.Auth
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Role { get; set; }
    }
}
