using Microsoft.AspNetCore.Identity;

namespace Vertical360.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        // TenantId null = platform-level user (SuperAdmin)
        public string? TenantId { get; set; }
    }
}
