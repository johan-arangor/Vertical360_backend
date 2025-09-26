using System.ComponentModel.DataAnnotations;

namespace Vertical360_back.Models
{
    public class CreateCompany
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Name { get; set; } = null!;
    }
}
