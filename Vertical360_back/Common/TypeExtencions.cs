using Microsoft.AspNetCore.Identity;
using Vertical360_back.Domain.Entityes;

namespace Vertical360_back.Common
{
    public static class TypeExtencions
    {
        public static bool NotValidationTenant(this Type type)
        {
            var booleans = new List<bool>()
                { 
                    type.IsAssignableFrom(typeof(IdentityRole)),
                    type.IsAssignableFrom(typeof(IdentityRoleClaim<string>)),
                    type.IsAssignableFrom(typeof(IdentityUser)),
                    type.IsAssignableFrom(typeof(IdentityUserLogin<string>)),
                    type.IsAssignableFrom(typeof(IdentityUserRole<string>)),
                    type.IsAssignableFrom(typeof(IdentityUserToken<string>)),
                    type.IsAssignableFrom(typeof(IdentityUserClaim<string>)),
                    typeof(IEntityCommon).IsAssignableFrom(type)
                };

            var result = booleans.Aggregate((a, b) => a || b);

            return result;
        }
    }
}
