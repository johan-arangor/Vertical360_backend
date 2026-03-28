using Vertical360.Core.Entities;

namespace Vertical360.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<IEnumerable<ApplicationUser>> GetAllAsync(string tenantId);
        Task CreateAsync(ApplicationUser user, string password, string role, string tenantId);
    }
}
