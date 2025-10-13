using Vertical360_back.Domain.Enums;

namespace Vertical360_back.Application.Contracts.Resident
{
    public class ResidentResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public DocumentTypeEnum DocumentType { get; set; }
        public bool IsOwner { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderTypeEnum? GenderType { get; set; }
    }
}
