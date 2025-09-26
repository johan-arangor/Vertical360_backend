using Microsoft.AspNetCore.Authorization;
using Vertical360_back.Domain.Entityes;
using Vertical360_back.Services;

namespace Vertical360_back.Security
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(Permissions permissions)
        {
            Permissions = permissions;
        }

        public Permissions Permissions
        {
            get
            {
                // TienePermisoProductos_Crear
                if (Enum.TryParse(typeof(Permissions),
                    Policy!.Substring(Constants.PrefixPolicy.Length),
                    ignoreCase: true, out var permiso))
                {
                    return (Permissions)permiso!;
                }

                return Permissions.Null;
            }
            set
            {
                Policy = $"{Constants.PrefixPolicy}{value.ToString()}";
            }
        }
    }
}
