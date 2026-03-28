using Microsoft.AspNetCore.Identity;

namespace Vertical360.Core.Entities
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        // CompanyId para saber en qué compañía aplica el role asignado
        public Guid? CompanyId { get; set; }
    }
}
