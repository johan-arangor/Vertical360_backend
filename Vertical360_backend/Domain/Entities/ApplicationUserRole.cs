using Microsoft.AspNetCore.Identity;

namespace Vertical360_backend.Domain.Entities
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        // CompanyId para saber en qué compañía aplica el role asignado
        public Guid? CompanyId { get; set; }
    }
}
