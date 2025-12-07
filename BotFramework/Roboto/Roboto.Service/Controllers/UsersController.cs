namespace Roboto.Service.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Roboto.Models.Dto;
    using Roboto.Repository;
    using Roboto.Models;
    using Roboto.Service.Auth;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;

    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public UsersController(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        [AllowAnonymous]
        [HttpPost("api/auth/register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
                // basic validation
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("username, email and password are required");

            var exists = await _userRepository.UsernameOrEmailExistsAsync(dto.Username, dto.Email);
            if (exists) return Conflict("username or email already in use");

            var (hash, salt) = _passwordHasher.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = hash,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Salt = salt,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddUserAsync(user);

            return Created($"/api/users/{user.Id}", new { user.Id, user.Username, user.Email });
        }

        [AllowAnonymous]
        [HttpPost("api/auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userRepository.GetUserByUsernameOrEmailAsync(dto.UsernameOrEmail);

            if (user == null) return Unauthorized();

            if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash, user.Salt))
                return Unauthorized();

            var token = _jwtService.GenerateToken(user);

            return Ok(new { token });
        }
    }
}