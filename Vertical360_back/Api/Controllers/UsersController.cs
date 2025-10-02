using Microsoft.AspNetCore.Mvc;
using Vertical360_back.Application.Contracts.Auth;
using Vertical360_back.Application.Interfaces.Services;
using Vertical360_back.Models;

namespace Vertical360_back.Api.Controllers
{
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResultDto), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _authService.LoginAsync(model.Email, model.Password, model.RememberMe);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Success = false, ErrorMessage = ex.Message });
            }
        }
    }
}
