using Microsoft.AspNetCore.Mvc;
using Vertical360_back.Application.Common.Errors;
using Vertical360_back.Application.Common.Responses;
using Vertical360_back.Application.Contracts.Auth;
using Vertical360_back.Application.Interfaces.Services;

namespace Vertical360_back.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class UsersController : BaseController
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse
                {
                    Code = "VALIDATION_ERROR",
                    Message = ErrorCatalog.Messages["VALIDATION_ERROR"]
                });

            try
            {
                var result = await _authService.LoginAsync(model);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
