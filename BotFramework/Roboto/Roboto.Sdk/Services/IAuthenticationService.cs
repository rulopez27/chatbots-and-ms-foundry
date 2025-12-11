using Roboto.Dtos;

namespace Roboto.Sdk.Services;

/// <summary>
/// Service for handling authentication with the Roboto API
/// </summary>
public interface IAuthenticationService
    {
    /// <summary>
    /// Login with username/email and password
    /// </summary>
    Task<LoginResponseDto> LoginAsync(LoginDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Register a new user
    /// </summary>
    Task<RegisterResponseDto> RegisterAsync(RegisterDto request, CancellationToken cancellationToken = default);        /// <summary>
        /// Get the current authentication token
        /// </summary>
        string? GetToken();

        /// <summary>
        /// Set the authentication token
        /// </summary>
        void SetToken(string token);

        /// <summary>
        /// Clear the authentication token
        /// </summary>
        void ClearToken();

        /// <summary>
        /// Check if currently authenticated
    /// </summary>
    bool IsAuthenticated();
}