using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Vertical360_back.Domain.Enums;

namespace Vertical360_back.Domain.Entityes.Tenants
{
    public class Resident : BaseEntity, IEntityTenant
    {
        public string TenantId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [MaxLength(50)]
        public string MiddleName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        [MaxLength(50)]
        public string MiddleLastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Document { get; set; } = null!;

        [Required]
        public DocumentTypeEnum DocumentType { get; set; }

        [Required]
        public Boolean IsActive { get; set; }

        [Required]
        public bool IsOwner { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string FullName => $"{FirstName} {MiddleName} {LastName} {MiddleLastName}".Replace("  ", " ").Trim();

        #region relations
        public string UserIdentityId { get; set; } = null!;
        public IdentityUser UserIdentity { get; set; } = null!;
        public Guid AdditionalInfoId { get; set; }
        public ResidentAdditionalInfo? AdditionalInfo { get; set; }
        #endregion
    }
}
