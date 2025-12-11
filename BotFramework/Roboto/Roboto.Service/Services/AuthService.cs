using Roboto.Dtos;
using Roboto.Models;
using Roboto.Repository;
using Roboto.Service.Auth;

namespace Roboto.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<(bool Success, string? ErrorMessage, User? User)> RegisterUserAsync(RegisterDto dto)
        {
            // Check if user already exists
            var exists = await _userRepository.UsernameOrEmailExistsAsync(dto.Username, dto.Email);
            if (exists)
            {
                return (false, "Username or email already in use", null);
            }

            // Hash the password
            var (hash, salt) = _passwordHasher.HashPassword(dto.Password);

            // Create the user entity
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

            // Persist to database
            await _userRepository.AddUserAsync(user);

            return (true, null, user);
        }

        public async Task<(bool Success, string? Token)> AuthenticateUserAsync(LoginDto dto)
        {
            // Find user by username or email
            var user = await _userRepository.GetUserByUsernameOrEmailAsync(dto.UsernameOrEmail);
            
            if (user == null)
            {
                return (false, null);
            }

            // Verify password
            if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash, user.Salt))
            {
                return (false, null);
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            return (true, token);
        }
    }
}
