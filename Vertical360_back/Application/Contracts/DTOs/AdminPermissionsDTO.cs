namespace Vertical360_back.Application.Contracts.DTOs
{
    public class AdminPermissionsDTO
    {
        public string UserId { get; set; } = null!;
        public string? Email { get; set; }
        public List<PermissionUserDTO> Permissions { get; set; } = new List<PermissionUserDTO>();
    }
}
