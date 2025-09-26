using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Vertical360_back.Common;
using Vertical360_back.Domain.Entityes;

namespace Vertical360_back.Security
{
    public static class IAuthorizationServiceExtensions
    {
        public static async Task<bool> HasPermission(this IAuthorizationService authorizationService, ClaimsPrincipal user, Permissions permission)
        {
            if (!user.Identity!.IsAuthenticated)
            {
                return false;
            }

            var namePolicy = $"{Constants.PrefixPolicy}{permission}";
            var result = await authorizationService.AuthorizeAsync(user, namePolicy);

            return result.Succeeded;
        }
    }
}
