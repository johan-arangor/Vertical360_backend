using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace Vertical360_back.Domain.Entityes
{
    public class Connection : IEntityCommon
    {
        public int Id { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; } = null!;
        public StatusLinkEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Companies Company { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;
    }
}
