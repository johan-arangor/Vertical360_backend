using System.ComponentModel.DataAnnotations;

namespace Vertical360_back.Models
{
    public class BaseModel
    {
        [Key]
        public Guid Id { get; set; }
    }
}
