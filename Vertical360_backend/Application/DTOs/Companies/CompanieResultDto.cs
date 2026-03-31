namespace Vertical360_backend.Application.DTOs.Companies
{
    public class CompanieResultDto
    {
        public Guid Id { get; set; }
        public string Nit { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? MobilePhone { get; set; }
        public string? Address { get; set; }

        // Ubicación — devuelve nombres resueltos para facilitar la UI
        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public Guid? CityId { get; set; }
        public string? CityName { get; set; }
        public string? PostalCode { get; set; }

        // Representante legal
        public string? LegalRepresentativeName { get; set; }
        public string? LegalRepresentativeEmail { get; set; }
        public string? LegalRepresentativePhone { get; set; }
        public string? LegalRepresentativeMobile { get; set; }

        // Administrador
        public string AdminName { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public string? AdminPhone { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

