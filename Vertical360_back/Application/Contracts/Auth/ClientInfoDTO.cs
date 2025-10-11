namespace Vertical360_back.Application.Contracts.Auth
{
    public class ClientInfoDto
    {
        public Guid CompanyId { get; set; }
        public string Name { get; set; } = null!;
    }
}
