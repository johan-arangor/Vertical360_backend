namespace Vertical360.Core.Entities
{
    public class LinkUserCompany
    {
        public string UserId { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }
        public Companies? Company { get; set; }
    }
}
