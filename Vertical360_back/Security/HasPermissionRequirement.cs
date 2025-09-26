using Microsoft.AspNetCore.Authorization;
using Vertical360_back.Entityes;

namespace Vertical360_back.Security
{
    public class HasPermissionRequirement : IAuthorizationRequirement
    {
        public HasPermissionRequirement(Permissions permission)
        {
            Permission = permission;
        }

        public Permissions Permission { get; }
    }
}
