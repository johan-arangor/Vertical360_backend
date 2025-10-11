using System.ComponentModel.DataAnnotations;
using Vertical360_back.Domain.ValueObjects;

namespace Vertical360_back.Domain.Enums
{
    public enum Permissions
    {
        [Hide]
        Null = 0, //Permiso que todos los usuarios deben tener si pertenecen a una empresa. Solo se elimina al desvincular un usuario de una empresa
        [Display(Description = "Puede crear")]
        Product_Create = 1,
        [Display(Description = "Puede Leer")]
        Product_Read = 2,
        [Display(Description = "Puede vincular usuarios")]
        User_Link = 3,
        [Display(Description = "Puede leer permisos")]
        Permission_Read = 4,
        [Display(Description = "Puede actualizar permisos")]
        Permission_Update = 5
    }
}
