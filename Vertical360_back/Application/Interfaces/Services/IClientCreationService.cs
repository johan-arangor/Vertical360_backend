using Vertical360_back.Application.Contracts.Client;
using Vertical360_back.Domain.Entityes.Common;

namespace Vertical360_back.Application.Interfaces.Services
{
    public interface IClientCreationService
    {
        Task CreateClientAndSeedAsync(string clientName, string adminEmail, string adminPassword);
        Task<Companies> CreateNewClientAsync(ClientCreationRequestDto request);
    }
}
