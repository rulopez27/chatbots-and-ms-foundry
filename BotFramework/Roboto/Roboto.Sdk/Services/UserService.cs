using System.Net.Http.Json;
using Roboto.Sdk.Exceptions;
using Roboto.Models.Dto;

namespace Roboto.Sdk.Services
{
    internal class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserDto> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/users/{id}", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new RobotoNotFoundException($"User with ID {id} not found");
            }

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new RobotoApiException(
                    $"Failed to get user: {response.StatusCode}", 
                    response.StatusCode, 
                    content);
            }

            return (await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken))!;
        }
    }
}
