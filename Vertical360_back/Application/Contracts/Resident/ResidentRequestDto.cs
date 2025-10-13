using System.ComponentModel.DataAnnotations;
using Vertical360_back.Domain.Enums;

namespace Vertical360_back.Application.Contracts.Resident
{
    public class ResidentRequestDto
    {
        [Required, MaxLength(50)]
        public string FirtsName { get; set; } = null!;

        [MaxLength(50)]
        public string? MiddleName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; } = null!;

        [MaxLength(50)]
        public string? MiddleLastName { get; set; }

        [Required, MaxLength(20)]
        public string Document { get; set; } = null!;

        [Required]
        public DocumentTypeEnum DocumentType { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool IsOwner { get; set; }

        [Required]
        public bool IsUser { get; set; }

        public ResidentAdditionalInfoDto? AdditionalInfo { get; set; }
    }
}
