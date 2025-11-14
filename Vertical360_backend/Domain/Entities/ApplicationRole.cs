using Microsoft.AspNetCore.Identity;

namespace Vertical360_backend.Domain.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    }
}
