using System.ComponentModel.DataAnnotations;

namespace Vertical360_backend.Application.DTOs.Companies
{
    /// <summary>
    /// Payload para crear una nueva unidad residencial (tenant).
    /// </summary>
    public class CompanyRequestDto
    {
        // ── Identificación ───────────────────────────────────────────────────
        [Required(ErrorMessage = "El NIT es obligatorio.")]
        [MaxLength(20)]
        public string Nit { get; set; } = null!;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "La razón social es obligatoria.")]
        [MaxLength(300)]
        public string BusinessName { get; set; } = null!;

        // ── Contacto de la unidad ────────────────────────────────────────────
        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(20)]
        public string? MobilePhone { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        // ── Ubicación ────────────────────────────────────────────────────────
        public Guid? CountryId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? CityId { get; set; }

        [MaxLength(10)]
        public string? PostalCode { get; set; }

        // ── Representante legal ──────────────────────────────────────────────
        [MaxLength(200)]
        public string? LegalRepresentativeName { get; set; }

        [EmailAddress]
        [MaxLength(200)]
        public string? LegalRepresentativeEmail { get; set; }

        [MaxLength(20)]
        public string? LegalRepresentativePhone { get; set; }

        [MaxLength(20)]
        public string? LegalRepresentativeMobile { get; set; }

        // ── Administrador inicial ────────────────────────────────────────────
        [Required(ErrorMessage = "El nombre del administrador es obligatorio.")]
        [MaxLength(200)]
        public string AdminName { get; set; } = null!;

        [Required(ErrorMessage = "El email del administrador es obligatorio.")]
        [EmailAddress]
        [MaxLength(200)]
        public string AdminEmail { get; set; } = null!;

        [MaxLength(20)]
        public string? AdminPhone { get; set; }

        /// <summary>
        /// Contraseña inicial del administrador.
        /// Si no se provee, se genera automáticamente y se envía por email.
        /// </summary>
        [MaxLength(100)]
        public string? AdminPassword { get; set; }
    }

    /// <summary>
    /// Payload para actualizar datos de una unidad existente.
    /// No permite cambiar NIT ni TenantKey.
    /// </summary>
    public class CompanyUpdateDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(300)]
        public string? BusinessName { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(20)]
        public string? MobilePhone { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        public Guid? CountryId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? CityId { get; set; }

        [MaxLength(10)]
        public string? PostalCode { get; set; }

        [MaxLength(200)]
        public string? LegalRepresentativeName { get; set; }

        [EmailAddress]
        [MaxLength(200)]
        public string? LegalRepresentativeEmail { get; set; }

        [MaxLength(20)]
        public string? LegalRepresentativePhone { get; set; }

        [MaxLength(20)]
        public string? LegalRepresentativeMobile { get; set; }

        [MaxLength(200)]
        public string? AdminName { get; set; }

        [EmailAddress]
        [MaxLength(200)]
        public string? AdminEmail { get; set; }

        [MaxLength(20)]
        public string? AdminPhone { get; set; }
    }
}

