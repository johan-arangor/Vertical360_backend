namespace Vertical360.Core.Entities
{
    public class City : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Acronym { get; set; }     // Ej: "MED"
        public string? Code { get; set; }        // Ej: "4" (indicativo local)

        public Guid DepartmentId { get; set; }
        public required Department Department { get; set; }
    }
}