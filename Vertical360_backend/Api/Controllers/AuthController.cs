using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Vertical360_backend.Application.DTOs.Auth;
using Vertical360_backend.Application.Interfaces;

namespace Vertical360_backend.Api.Controllers
{
    /// <summary>
    /// Autenticación y gestión de contraseñas.
    /// El flujo de login es de dos pasos:
    /// 1. POST /login → devuelve un pre_auth token y la lista de unidades del usuario.
    /// 2. POST /select-tenant → devuelve el JWT final con el tenant seleccionado.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IPasswordResetService _passwordResetService;

        public AuthController(
            IAuthService authService,
            ITokenService tokenService,
            IPasswordResetService passwordResetService)
        {
            _authService = authService;
            _tokenService = tokenService;
            _passwordResetService = passwordResetService;
        }

        /// <summary>
        /// Paso 1 del login. Valida credenciales y devuelve un pre_auth token (2 min)
        /// junto con la lista de unidades a las que pertenece el usuario.
        /// </summary>
        [HttpPost("login")]
        [SwaggerOperation(
            Summary     = "Inicio de sesión (paso 1)",
            Description = "Valida usuario y contraseña. Devuelve un `pre_auth` token de corta duración " +
                          "y la lista de unidades residenciales asociadas al usuario. " +
                          "Usar el token y un `companyId` en **/select-tenant** para obtener el JWT definitivo.",
            OperationId = "Auth_Login")]
        [SwaggerResponse(200, "Credenciales válidas. Token pre_auth generado.", typeof(LoginResultDto))]
        [SwaggerResponse(400, "Body nulo o campos faltantes.")]
        [SwaggerResponse(401, "Credenciales inválidas.")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            if (model == null) return BadRequest(new { message = "El body es requerido." });

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
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        /// <summary>
        /// Paso 2 del login. Selecciona la unidad residencial (tenant) y emite el JWT definitivo.
        /// </summary>
        [HttpPost("select-tenant")]
        [SwaggerOperation(
            Summary     = "Selección de tenant (paso 2)",
            Description = "Recibe el `pre_auth` token del paso 1 y el `companyId` elegido por el usuario. " +
                          "Devuelve el JWT definitivo con el claim `tenant_id`. " +
                          "Este token debe enviarse como `Authorization: Bearer {token}` en todas las peticiones siguientes.",
            OperationId = "Auth_SelectTenant")]
        [SwaggerResponse(200, "JWT definitivo generado.", typeof(LoginResultDto))]
        [SwaggerResponse(400, "Body nulo.")]
        [SwaggerResponse(401, "Pre-auth token inválido, expirado o companyId no autorizado.")]
        public async Task<IActionResult> SelectTenant([FromBody] SelectTenantRequestDto model)
        {
            if (model == null) return BadRequest(new { message = "El body es requerido." });

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
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        /// <summary>
        /// Solicita el envío de un código OTP de 6 dígitos al email del usuario.
        /// Por seguridad, la respuesta es siempre la misma independientemente de si el email existe.
        /// </summary>
        [HttpPost("forgot-password")]
        [SwaggerOperation(
            Summary     = "Solicitar recuperación de contraseña",
            Description = "Envía un código OTP de 6 dígitos al email registrado. El código expira en 10 minutos. " +
                          "La respuesta es siempre 200 para no revelar si el email existe en el sistema.",
            OperationId = "Auth_ForgotPassword")]
        [SwaggerResponse(200, "Solicitud procesada (respuesta genérica por seguridad).")]
        [SwaggerResponse(500, "Error interno al enviar el email.")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto model)
        {
            try
            {
                await _passwordResetService.ForgotPasswordAsync(model);
                return Ok(new { message = "Si el correo existe, recibirás un código de verificación." });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        /// <summary>
        /// Restablece la contraseña usando el código OTP recibido por email.
        /// </summary>
        [HttpPost("reset-password")]
        [SwaggerOperation(
            Summary     = "Restablecer contraseña con OTP",
            Description = "Valida el OTP enviado por email y actualiza la contraseña del usuario. " +
                          "El OTP solo puede usarse una vez y expira a los 10 minutos.",
            OperationId = "Auth_ResetPassword")]
        [SwaggerResponse(200, "Contraseña actualizada correctamente.")]
        [SwaggerResponse(401, "Código OTP inválido o expirado.")]
        [SwaggerResponse(500, "Error al actualizar la contraseña.")]
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

        /// <summary>
        /// Endpoint de prueba para verificar que el servicio de email está funcionando.
        /// Solo disponible en desarrollo.
        /// </summary>
        [HttpGet("test-email")]
        [SwaggerOperation(
            Summary     = "Prueba de envío de email",
            Description = "Envía un email de prueba a la dirección especificada. Solo para uso en desarrollo.",
            OperationId = "Auth_TestEmail")]
        [SwaggerResponse(200, "Email enviado correctamente.")]
        [SwaggerResponse(500, "Error al enviar el email.")]
        public async Task<IActionResult> TestEmail(
            [FromServices] IEmailService emailService,
            [FromQuery, SwaggerParameter("Dirección de destino del email de prueba", Required = true)] string to)
        {
            await emailService.SendAsync(
                to: to,
                subject: "Test Vertical360",
                body: "<h1>Email funcionando ✅</h1>");
            return Ok(new { message = "Email enviado." });
        }
    }
}

