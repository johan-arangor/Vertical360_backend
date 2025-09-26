using System.ComponentModel.DataAnnotations;

namespace Vertical360_back.Application.Contracts.DTOs
{
    public class BaseModel
    {
        [Key]
        public Guid Id { get; set; }
    }
}
