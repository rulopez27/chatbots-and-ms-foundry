using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Roboto.Sdk.Configuration;
using Roboto.Sdk.Exceptions;
using Roboto.Dtos;

namespace Roboto.Sdk.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorage _tokenStorage;

        public AuthenticationService(HttpClient httpClient, ITokenStorage tokenStorage)
        {
            _httpClient = httpClient;
            _tokenStorage = tokenStorage;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new RobotoAuthenticationException("Login failed", content);
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken);
            if (loginResponse?.Token != null)
            {
                SetToken(loginResponse.Token);
            }

            return loginResponse!;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterDto request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new RobotoValidationException("Registration failed", content);
            }

            return (await response.Content.ReadFromJsonAsync<RegisterResponseDto>(cancellationToken))!;
        }

        public string? GetToken() => _tokenStorage.Token;

        public void SetToken(string token)
        {
            _tokenStorage.Token = token;
            // Token will be added to requests by AuthenticationHandler
        }

        public void ClearToken()
        {
            _tokenStorage.Token = null;
        }

        public bool IsAuthenticated() => !string.IsNullOrEmpty(_tokenStorage.Token);

        public int? AuthenticatedUserId
        {
            get
            {
                if (string.IsNullOrEmpty(_tokenStorage.Token))
                    return null;

                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(_tokenStorage.Token);
                    
                    // Try to get the user ID from the NameIdentifier claim
                    var userIdClaim = jwtToken.Claims.FirstOrDefault(c => 
                        c.Type == ClaimTypes.NameIdentifier || 
                        c.Type == "nameid" || 
                        c.Type == "sub" ||
                        c.Type == "userId");

                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        return userId;
                    }
                }
                catch
                {
                    // If token parsing fails, return null
                }

                return null;
            }
        }

        public string? AuthenticatedUsername
        {
            get
            {
                if (string.IsNullOrEmpty(_tokenStorage.Token))
                    return null;

                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(_tokenStorage.Token);
                    
                    // Try to get the username from the UniqueName claim
                    var usernameClaim = jwtToken.Claims.FirstOrDefault(c => 
                        c.Type == ClaimTypes.Name || 
                        c.Type == "unique_name" || 
                        c.Type == JwtRegisteredClaimNames.UniqueName);

                    return usernameClaim?.Value;
                }
                catch
                {
                    // If token parsing fails, return null
                }

                return null;
            }
        }
    }
}
