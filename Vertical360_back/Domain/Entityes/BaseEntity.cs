using System.ComponentModel.DataAnnotations;

namespace Vertical360_back.Domain.Entityes
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
