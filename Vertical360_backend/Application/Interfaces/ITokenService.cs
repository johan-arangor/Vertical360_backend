using System.Security.Claims;
using Vertical360_backend.Domain.Entities;

namespace Vertical360_backend.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user, Guid companyId);
    }
}
