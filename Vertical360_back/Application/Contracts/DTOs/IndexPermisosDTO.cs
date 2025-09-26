namespace Vertical360_back.Application.Contracts.DTOs
{
    public class IndexPermisosDTO
    {
        public string CompanyName { get; set; } = null!;
        public IEnumerable<UserDTO> Employeds { get; set; } = null!;
    }
}
