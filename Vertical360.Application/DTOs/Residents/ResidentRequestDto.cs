namespace Vertical360.Application.DTOs.Residents
{
    public class ResidentRequestDto
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; } = string.Empty;
        public string LastName { get; set; }
        public string SecondLastName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool IsOwner { get; set; }
    }
}
