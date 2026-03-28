using Vertical360.Core.Enums;

namespace Vertical360.Application.DTOs.Residents
{
    public class ResidentResultDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DocumentsTypeEnum DocumentType { get; set; }
        public string Document { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsOwner { get; set; }
        public bool IsActive { get; set; }
    }
}
