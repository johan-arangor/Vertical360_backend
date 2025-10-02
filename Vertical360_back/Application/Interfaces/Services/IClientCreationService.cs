namespace Vertical360_back.Application.Interfaces.Services
{
    public interface IClientCreationService
    {
        Task CreateClientAndSeedAsync(string clientName, string adminEmail, string adminPassword);
    }
}
