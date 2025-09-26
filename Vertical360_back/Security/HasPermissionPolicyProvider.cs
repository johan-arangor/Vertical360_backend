using Microsoft.AspNetCore.Authorization;
using Vertical360_back.Common;
using Vertical360_back.Domain.Entityes;

namespace Vertical360_back.Security
{
    public class HasPermissionPolicyProvider
    {
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(
                new AuthorizationPolicyBuilder("Identity.Application")
                .RequireAuthenticatedUser().Build());
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy?>(null!);
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(Constants.PrefixPolicy,
                StringComparison.OrdinalIgnoreCase) && Enum.TryParse(typeof(Permissions),
                policyName.Substring(Constants.PrefixPolicy.Length),
                out var permisionObj))
            {
                var permission = (Permissions)permisionObj!;
                var policy = new AuthorizationPolicyBuilder("Identity.Application");
                policy.AddRequirements(new HasPermissionRequirement(permission));
                return Task.FromResult<AuthorizationPolicy?>(policy.Build());
            }

            return Task.FromResult<AuthorizationPolicy?>(null!);
        }
    }
}
