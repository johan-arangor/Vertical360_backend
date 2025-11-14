using Vertical360_backend.Domain.Entities;

namespace Vertical360_backend.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<IEnumerable<ApplicationUser>> GetAllAsync(string tenantId);
        Task CreateAsync(ApplicationUser user, string password, string role, string tenantId);
    }
}
