using Microsoft.AspNetCore.Identity;

namespace Vertical360_back.Domain.Entityes
{
    public class CompanyUserPermissions : IEntityCommon
    {
        public string UserId { get; set; } = null!;
        public Guid CompanyId { get; set; }
        public Permissions Permissions { get; set; }
        public IdentityUser User { get; set; } = null!;
        public Companies? Companies { get; set; }
    }
}
