namespace Vertical360_back.Application.Contracts.Auth
{
    public class ClientInfoDTO
    {
        public Guid CompanyId { get; set; }
        public string Name { get; set; } = null!;
        public string ConnectionString { get; set; } = null!;
    }
}
