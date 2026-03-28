namespace Vertical360.Application.DTOs.Auth
{
    public class SelectTenantRequestDto
    {
        public string PreAuthToken { get; set; } = string.Empty;
        public string CompanyId { get; set; }
    }
}
