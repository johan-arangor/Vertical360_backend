using System.ComponentModel.DataAnnotations;
using Vertical360_back.Domain.Enums;

namespace Vertical360_back.Domain.Entityes.Tenants
{
    public class Resident : BaseEntity, IEntityTenant
    {
        public string TenantId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FirtsName { get; set; } = null!;

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
        [MaxLength(20)]
        public string Apartment { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string FullName => $"{FirtsName} {MiddleName} {LastName} {MiddleLastName}".Replace("  ", " ").Trim();
    }
}
