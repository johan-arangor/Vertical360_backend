using System.ComponentModel.DataAnnotations;

namespace Vertical360_back.Models
{
    public class RegistryViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
