using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model.Email, model.Password, model.RememberMe);

            if (!result.Success)
                return Unauthorized(new { error = result.ErrorMessage });

            return Ok(new { redirect = result.RedirectUrl, userId = result.UserId });
        }
    }
}
