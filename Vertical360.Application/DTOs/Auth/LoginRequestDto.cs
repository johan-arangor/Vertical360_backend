namespace Vertical360.Application.DTOs.Auth
{
    public class LoginRequestDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;

        // CompanyId opcional: si se incluye, el servidor generará el token para esa company/tenant
        public Guid? CompanyId { get; set; }
    }
}
