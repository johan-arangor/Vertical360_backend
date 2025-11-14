using System.Collections.Generic;

namespace Vertical360_backend.Domain.Entities
{
    public class Country : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Acronym { get; set; }     // Ej: "COL"
        public string? Code { get; set; }        // Ej: "+57" (indicativo)

        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}