using System.ComponentModel.DataAnnotations;

namespace Vertical360_back.Domain.Entityes.Common
{
    public class Cities : BaseEntity, IEntityCommon
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        [Required]
        [MaxLength(10)]
        public string Code { get; set; } = null!;

        #region relations
        public Guid DepartmentId { get; set; }
        public Departments Department { get; set; } = null!;
        #endregion
    }
}
