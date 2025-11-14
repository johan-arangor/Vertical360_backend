namespace Vertical360_backend.Domain.Entities
{
    public class Companies : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? TenantKey { get; set; } // si creas tenant DB
        public string? CreatedByUserId { get; set; }
    }
}
