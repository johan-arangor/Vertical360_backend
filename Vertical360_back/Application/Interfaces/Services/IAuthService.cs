using Vertical360_back.Application.Contracts.Auth;

namespace Vertical360_back.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResultDto> LoginAsync(LoginRequestDto model);
    }
}
