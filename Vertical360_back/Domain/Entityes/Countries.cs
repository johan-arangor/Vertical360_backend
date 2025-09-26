using System.ComponentModel.DataAnnotations;

namespace Vertical360_back.Domain.Entityes
{
    public class Countries : IEntityCommon
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        [Required]
        [StringLength(10)]
        public string Code { get; set; } = null!;
    }
}
