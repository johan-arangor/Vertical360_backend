using Vertical360_backend.Domain.Enums;

namespace Vertical360_backend.Domain.Entities
{
    public class Resident
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TenantId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string SecondName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string SecondLastName { get; set; } = null!;
        public DocumentsTypeEnum DocumentType { get; set; }
        public string Document { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public bool IsOwner { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string FullName
        {
            get
            {
                return $"{FirstName} {SecondName} {LastName} {SecondLastName}";
            }
        }
    }
}
