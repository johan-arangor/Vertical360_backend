using Microsoft.AspNetCore.Identity;
using Vertical360_back.Domain.Entityes;

namespace Vertical360_back.Domain.ValueObjects
{
    public static class TypeExtencions
    {
        public static bool NotValidationTenant(this Type type)
        {
            var exclusions = new List<bool>()
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

            return exclusions.Any(x => x);
        }
    }
}
