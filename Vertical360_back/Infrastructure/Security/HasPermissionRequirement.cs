using Microsoft.AspNetCore.Authorization;
using Vertical360_back.Domain.Enums;

namespace Vertical360_back.Infrastructure.Security
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
