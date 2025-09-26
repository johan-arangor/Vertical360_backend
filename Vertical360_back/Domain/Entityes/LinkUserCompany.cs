using Microsoft.AspNetCore.Identity;
using Vertical360_back.Domain.Enums;

namespace Vertical360_back.Domain.Entityes
{
    public class LinkUserCompany : IEntityCommon
    {
        public int Id {  get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; } = null!;
        public StatusLinkEnum statusLink { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public Companies Companies { get; set; } = null!;
        public IdentityUser UserIdentity { get; set; } = null!;
    }
}
