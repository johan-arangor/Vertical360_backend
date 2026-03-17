using Microsoft.AspNetCore.Mvc;
using Vertical360_backend.Application.DTOs.Auth;
using Vertical360_backend.Application.Interfaces;

namespace Vertical360_backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IPasswordResetService _passwordResetService;

        public AuthController(IAuthService authService, ITokenService tokenService, IPasswordResetService passwordResetService)
        {
            _authService = authService;
            _tokenService = tokenService;
            _passwordResetService = passwordResetService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            if (model == null) return BadRequest("Request body is required.");

            try
            {
                var result = await _authService.LoginAsync(model);
                if (!result.Success) return Unauthorized();
                return Ok(result);
            }
            catch (UnauthorizedAccessException uex)
            {
                return Unauthorized(new { message = uex.Message });
            }
            catch (Exception ex)
            {
                // Return 400 for client errors or 500 for others; keep simple and informative
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost("select-tenant")]
        public async Task<IActionResult> SelectTenant([FromBody] SelectTenantRequestDto model)
        {
            if (model == null) return BadRequest("Request body is required.");

            try
            {
                var result = await _authService.SelectTenantAsync(model);
                if (!result.Success) return Unauthorized();
                return Ok(result);
            }
            catch (UnauthorizedAccessException uex)
            {
                return Unauthorized(new { message = uex.Message });
            }
            catch (Exception ex)
            {
                // Return 400 for client errors or 500 for others; keep simple and informative
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto model)
        {
            try
            {
                await _passwordResetService.ForgotPasswordAsync(model);
                // Respuesta genérica para no revelar si el email existe
                return Ok(new { message = "Si el correo existe, recibirás un código de verificación." });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto model)
        {
            try
            {
                await _passwordResetService.ResetPasswordAsync(model);
                return Ok(new { message = "Contraseña actualizada correctamente." });
            }
            catch (UnauthorizedAccessException uex)
            {
                return Unauthorized(new { message = uex.Message });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet("test-email")]
        public async Task<IActionResult> TestEmail([FromServices] IEmailService emailService, [FromQuery] string to)
        {
            await emailService.SendAsync(
                to: to,
                subject: "Test Vertical360",
                body: "<h1>Email funcionando ✅</h1>"
            );
            return Ok("Email enviado.");
        }
    }
}
