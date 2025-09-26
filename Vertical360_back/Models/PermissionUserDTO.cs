using Vertical360_back.Domain.Entityes;

namespace Vertical360_back.Models
{
    public class PermissionUserDTO
    {
        public Permissions Permission { get; set; }
        public bool Assigned { get; set; }
        public string? Description { get; set; }
    }
}
