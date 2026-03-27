using Vertical360_backend.Application.DTOs.Auth;

namespace Vertical360_backend.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResultDto> LoginAsync(LoginRequestDto request);
        Task<LoginResultDto> SelectTenantAsync(SelectTenantRequestDto request);
    }
}
