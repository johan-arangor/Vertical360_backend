using System.ComponentModel.DataAnnotations;

namespace Vertical360_back.Domain.Enums
{
    public enum GenderTypeEnum
    {
        [Display(Description = "Femenino")]
        Female = 0,
        [Display(Description = "Masculino")]
        Male = 1
    }
}
