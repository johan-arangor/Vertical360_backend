namespace Vertical360_back.Application.Contracts.Auth
{
    public class LoginResultDto
    {
        public bool Success { get; set; }
        public string? UserId { get; set; }
        public string? RedirectUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
