using System.ComponentModel.DataAnnotations;

namespace Vertical360_back.Domain.Enums
{
    public enum DocumentTypeEnum
    {
        [Display(Name = "Cédula de Ciudadanía")]
        CC = 0,

        [Display(Name = "Cédula de Extranjería")]
        CE = 1,

        [Display(Name = "Pasaporte")]
        PA = 2,

        [Display(Name = "NIT")]
        NIT = 3
    }
}
