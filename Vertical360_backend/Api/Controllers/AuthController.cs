using Microsoft.AspNetCore.Mvc;
using Vertical360_backend.Application.DTOs.Auth;
using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Infrastructure.Auth;
using Vertical360_backend.Infrastructure.Services;

namespace Vertical360_backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly ITokenService _token;

        public AuthController(IAuthService auth, ITokenService token)
        {
            _auth = auth;
            _token = token;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var res = await _auth.LoginAsync(model);
            return Ok(res);
        }
    }
}
