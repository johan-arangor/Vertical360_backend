using Vertical360.Application.DTOs.Auth;

namespace Vertical360.Application.Interfaces
{
    public interface IPasswordResetService
    {
        Task ForgotPasswordAsync(ForgotPasswordRequestDto model);
        Task ResetPasswordAsync(ResetPasswordRequestDto model);
    }
}
