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
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$"", ErrorMessage = ""El número de teléfono no es válido.")]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "El teléfono móvil es obligatorio.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$"", ErrorMessage = ""El número de teléfono no es válido.")]
        [MaxLength(20)]
        public string? MobilePhone { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [MaxLength(300)]
        public string? Address { get; set; }

        // ── Ubicación ────────────────────────────────────────────────────────
        [Required(ErrorMessage = "El país es obligatorio.")]
        public string Country { get; set; } = null!;
        [Required(ErrorMessage = "El departamento es obligatorio.")]
        public string Department { get; set; } = null!;
        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        public string City { get; set; } = null!;

        [MaxLength(10)]
        public string? PostalCode { get; set; }

        // ── Representante legal ──────────────────────────────────────────────
        [Required(ErrorMessage = "El nombre del representante legal es obligatorio.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre del representante legal solo puede contener letras y espacios.")]
        [MaxLength(200)]
        public string? LegalRepresentativeName { get; set; }

        [Required(ErrorMessage = "El email del representante legal es obligatorio.")]
        [EmailAddress]
        [MaxLength(200)]
        public string? LegalRepresentativeEmail { get; set; }

        [Required(ErrorMessage = "El teléfono del representante legal es obligatorio.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$"", ErrorMessage = ""El número de teléfono no es válido.")]
        [MaxLength(20)]
        public string? LegalRepresentativePhone { get; set; }

        [Required(ErrorMessage = "El teléfono móvil del representante legal es obligatorio.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$"", ErrorMessage = ""El número de teléfono no es válido.")]
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

        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "El número de teléfono no es válido.")]
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
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(200)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "La razón social es obligatoria.")]
        [MaxLength(300)]
        public string? BusinessName { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "El número de teléfono no es válido.")]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "El teléfono móvil es obligatorio.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "El número de teléfono no es válido.")]
        [MaxLength(20)]
        public string? MobilePhone { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [MaxLength(300)]
        public string? Address { get; set; }

        [Required(ErrorMessage = "El país es obligatorio.")]
        public string Country { get; set; } = null!;
        [Required(ErrorMessage = "El departamento es obligatorio.")]
        public string Department { get; set; } = null!;
        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "El código postal es obligatorio.")]
        [MaxLength(10)]
        public string? PostalCode { get; set; }

        [Required(ErrorMessage = "El nombre del representante legal es obligatorio.")]
        [MaxLength(200)]
        public string? LegalRepresentativeName { get; set; }

        [Required(ErrorMessage = "El email del representante legal es obligatorio.")]
        [EmailAddress]
        [MaxLength(200)]
        public string? LegalRepresentativeEmail { get; set; }

        [Required(ErrorMessage = "El teléfono del representante legal es obligatorio.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "El número de teléfono no es válido.")]
        [MaxLength(20)]
        public string? LegalRepresentativePhone { get; set; }

        [Required(ErrorMessage = "El teléfono móvil del representante legal es obligatorio.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "El número de teléfono no es válido.")]
        [MaxLength(20)]
        public string? LegalRepresentativeMobile { get; set; }

        [Required(ErrorMessage = "El nombre del administrador es obligatorio.")]
        [MaxLength(200)]
        public string? AdminName { get; set; }

        [Required(ErrorMessage = "El email del administrador es obligatorio.")]
        [EmailAddress]
        [MaxLength(200)]
        public string? AdminEmail { get; set; }

        [Required(ErrorMessage = "El teléfono del administrador es obligatorio.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "El número de teléfono no es válido.")]
        [MaxLength(20)]
        public string? AdminPhone { get; set; }
    }
}

