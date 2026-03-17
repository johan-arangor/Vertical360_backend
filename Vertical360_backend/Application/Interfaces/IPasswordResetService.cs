using Vertical360_backend.Application.DTOs.Auth;

namespace Vertical360_backend.Application.Interfaces
{
    public interface IPasswordResetService
    {
        Task ForgotPasswordAsync(ForgotPasswordRequestDto model);
        Task ResetPasswordAsync(ResetPasswordRequestDto model);
    }
}
