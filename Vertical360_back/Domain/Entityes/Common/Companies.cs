using Microsoft.AspNetCore.Identity;

namespace Vertical360_back.Domain.Entityes.Common
{
    public class Companies : IEntityCommon
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        #region relations
        public string? UserCreationId { get; set; }
        public IdentityUser UserCreation { get; set; } = null!;
        public List<CompanyUserPermissions> CompanyUserPermissions { get; set; } = null!;
        #endregion
    }
}
