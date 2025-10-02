using System.ComponentModel.DataAnnotations;

namespace Vertical360_back.Domain.Entityes
{
    public class Products : BaseEntity, IEntityTenant
    {
        [Required]
        string IEntityTenant.TenantId { get; set; } = null!;
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;
        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = null!;
        [Required]
        [StringLength(100)]
        public decimal Price { get; set; } = 0;
        [Required]
        [StringLength(100)]
        public int Stock { get; set; } = 0;
        [Required]
        [StringLength(100)]
        public string Category { get; set; } = null!;
        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = null!;
    }
}
