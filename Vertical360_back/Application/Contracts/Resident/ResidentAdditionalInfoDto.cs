using Vertical360_back.Domain.Enums;

namespace Vertical360_back.Application.Contracts.Resident
{
    public class ResidentAdditionalInfoDto
    {
        public DateTime DateOfBirth { get; set; }
        public GenderTypeEnum GenderType { get; set; }
    }
}
