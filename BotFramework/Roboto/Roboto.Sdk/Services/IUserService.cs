using Roboto.Dtos;

namespace Roboto.Sdk.Services;

/// <summary>
/// Service for managing users
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Get user by ID
    /// </summary>
    Task<UserDto> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
}
