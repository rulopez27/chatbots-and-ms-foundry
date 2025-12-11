using Roboto.Dtos;
using Roboto.Models;

namespace Roboto.Service.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string? ErrorMessage, User? User)> RegisterUserAsync(RegisterDto dto);
        Task<(bool Success, string? Token)> AuthenticateUserAsync(LoginDto dto);
    }
}
