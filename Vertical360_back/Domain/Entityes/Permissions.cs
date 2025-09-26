using System.ComponentModel.DataAnnotations;
using Vertical360_back.Services;

namespace Vertical360_back.Domain.Entityes
{
    public enum Permissions
    {
        [Hide]
        Null = 0, //Permiso que todos los usuarios deben tener si pertenecen a una empresa. Solo se elimina al desvincular un usuario de una empresa
        [Display(Description ="Puede crear")]
        Product_Create = 1,
        [Display(Description ="Puede Leer")]
        Proctuct_Read = 2,
        User_Link = 3,
        Permission_Read = 4,
        Permission_Update = 5
    }
}
