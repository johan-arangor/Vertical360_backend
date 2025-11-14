using System.Collections.Generic;

namespace Vertical360_backend.Domain.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Acronym { get; set; }     // Ej: "ANT"
        public string? Code { get; set; }        // Ej: "4" (indicativo local)

        public Guid CountryId { get; set; }
        public required Country Country { get; set; }

        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}