 using Microsoft.AspNetCore.Mvc;
 using Roboto.Dtos;
 using Roboto.Repository;
 using Roboto.Models;
 using Roboto.Service.Auth;
 using Microsoft.AspNetCore.Authorization;
 using AutoMapper;
 using Roboto.Service.Extensions;

namespace Roboto.Service.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILinkService _linkService;
        private readonly LinkGenerator _linkGenerator;

        public AuthController(IUserRepository userRepository, 
            IPasswordHasher passwordHasher, 
            IJwtService jwtService, 
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILinkService linkService,
            LinkGenerator linkGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _linkService = linkService;
            _linkGenerator = linkGenerator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
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

        [HttpPost("login")]
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