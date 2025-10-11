using System.ComponentModel.DataAnnotations;

namespace Vertical360_back.Domain.Entityes.Common
{
    public class Departments : BaseEntity, IEntityCommon
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        [Required]
        [MaxLength(10)]
        public string Code { get; set; } = null!;

        #region relations
        public Guid CountryId { get; set; }
        public Countries Country { get; set; } = null!;

        public ICollection<Cities> Cities { get; set; } = new List<Cities>();
        #endregion
    }
}
