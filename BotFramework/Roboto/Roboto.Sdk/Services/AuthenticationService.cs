using System.Net.Http.Json;
using Roboto.Sdk.Configuration;
using Roboto.Sdk.Exceptions;
using Roboto.Dtos;

namespace Roboto.Sdk.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private string? _token;

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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

        public string? GetToken() => _token;

        public void SetToken(string token)
        {
            _token = token;
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearToken()
        {
            _token = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public bool IsAuthenticated() => !string.IsNullOrEmpty(_token);
    }
}
