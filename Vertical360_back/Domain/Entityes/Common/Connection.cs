using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using Vertical360_back.Domain.Enums;

namespace Vertical360_back.Domain.Entityes.Common
{
    public class Connection : BaseEntity, IEntityCommon
    {
        public Guid CompanyId { get; set; }
        public string UserId { get; set; } = null!;
        public StatusLinkEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Companies Company { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;
    }
}
