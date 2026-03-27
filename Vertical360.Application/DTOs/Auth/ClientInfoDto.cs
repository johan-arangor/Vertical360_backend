namespace Vertical360.Application.DTOs.Auth
{
    public class ClientInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? TenantConnectionString { get; set; }
    }
}