 using Microsoft.AspNetCore.Mvc;
 using Roboto.Dtos;
 using Microsoft.AspNetCore.Authorization;
 using Roboto.Service.Services;

namespace Roboto.Service.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var (success, errorMessage, user) = await _authService.RegisterUserAsync(dto);

            if (!success)
            {
                return Conflict(errorMessage);
            }

            return Created($"/api/users/{user!.Id}", new { user.Id, user.Username, user.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var (success, token) = await _authService.AuthenticateUserAsync(dto);

            if (!success)
            {
                return Unauthorized();
            }

            return Ok(new { token });
        }
    }
}