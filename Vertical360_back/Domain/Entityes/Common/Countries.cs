using System.ComponentModel.DataAnnotations;

namespace Vertical360_back.Domain.Entityes.Common
{
    public class Countries : BaseEntity, IEntityCommon
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        [Required]
        [StringLength(10)]
        public string Code { get; set; } = null!;
        [Required]
        [StringLength(4)]
        public string PhoneCode { get; set; } = null!;

        #region relations
        public ICollection<Departments> Departments { get; set; } = new List<Departments>();
        #endregion
    }
}
