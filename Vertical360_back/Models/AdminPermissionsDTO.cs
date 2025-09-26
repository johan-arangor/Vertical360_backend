namespace Vertical360_back.Models
{
    public class AdminPermissionsDTO
    {
        public string UserId { get; set; } = null!;
        public string? Email { get; set; }
        public List<PermissionUserDTO> Permissions { get; set; } = new List<PermissionUserDTO>();
    }
}
