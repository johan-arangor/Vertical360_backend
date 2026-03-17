namespace Vertical360_backend.Domain.Entities
{
    /// <summary>
    /// Representa una unidad residencial (cliente/tenant) en la plataforma.
    /// Cada instancia corresponde a una propiedad horizontal independiente.
    /// </summary>
    public class Companies : BaseEntity
    {
        // ── Identificación de la unidad ──────────────────────────────────────
        public string Nit { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string BusinessName { get; set; } = null!;       // Razón social

        // ── Contacto de la unidad ────────────────────────────────────────────
        public string? Phone { get; set; }                       // Teléfono fijo
        public string? MobilePhone { get; set; }                 // Celular
        public string? Address { get; set; }

        // ── Ubicación geográfica ─────────────────────────────────────────────
        public Guid? CountryId { get; set; }
        public Country? Country { get; set; }

        public Guid? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public Guid? CityId { get; set; }
        public City? City { get; set; }

        public string? PostalCode { get; set; }

        // ── Representante legal ──────────────────────────────────────────────
        public string? LegalRepresentativeName { get; set; }
        public string? LegalRepresentativeEmail { get; set; }
        public string? LegalRepresentativePhone { get; set; }
        public string? LegalRepresentativeMobile { get; set; }

        // ── Administrador de la unidad ───────────────────────────────────────
        public string AdminName { get; set; } = null!;
        public string AdminEmail { get; set; } = null!;
        public string? AdminPhone { get; set; }

        // ── Metadata del tenant ──────────────────────────────────────────────
        public string? TenantKey { get; set; }
        public string? CreatedByUserId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

