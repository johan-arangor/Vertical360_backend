namespace Vertical360.Application.DTOs.Auth
{
    public class LoginResultDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool Success { get; set; } = false;
        public List<ClientInfoDto> AssociatedClients { get; set; } = new();
    }
}
