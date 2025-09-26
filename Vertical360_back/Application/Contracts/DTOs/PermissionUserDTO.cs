using Vertical360_back.Domain.Enums;

namespace Vertical360_back.Application.Contracts.DTOs
{
    public class PermissionUserDTO
    {
        public Permissions Permission { get; set; }
        public bool Assigned { get; set; }
        public string? Description { get; set; }
    }
}
