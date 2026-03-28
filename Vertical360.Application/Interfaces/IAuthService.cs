using Vertical360.Application.DTOs.Auth;

namespace Vertical360.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResultDto> LoginAsync(LoginRequestDto request);
        Task<LoginResultDto> SelectTenantAsync(SelectTenantRequestDto request);
    }
}
