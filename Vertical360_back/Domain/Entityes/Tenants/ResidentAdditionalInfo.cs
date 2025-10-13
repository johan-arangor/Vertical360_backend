using Vertical360_back.Domain.Enums;

namespace Vertical360_back.Domain.Entityes.Tenants
{
    public class ResidentAdditionalInfo : BaseEntity, IEntityTenant
    {
        public string TenantId { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public GenderTypeEnum GenderType { get; set; }

        #region relation
        public Resident? Resident { get; set; }
        #endregion
    }
}
