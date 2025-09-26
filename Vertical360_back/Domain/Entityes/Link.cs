using Microsoft.AspNetCore.Identity;
using Vertical360_back.Domain.Enums;

namespace Vertical360_back.Domain.Entityes
{
    public class Link : IEntityCommon
    {
        public int Id { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; } = null!;
        public StatusLinkEnum Status { get; set; }
        public DateTime DateCreate { get; set; }
        public Companies Companie { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;
    }
}
