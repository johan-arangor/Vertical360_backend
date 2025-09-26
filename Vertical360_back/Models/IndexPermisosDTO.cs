namespace Vertical360_back.Models
{
    public class IndexPermisosDTO
    {
        public string CompanyName { get; set; } = null!;
        public IEnumerable<UserDTO> Employeds { get; set; } = null!;
    }
}
