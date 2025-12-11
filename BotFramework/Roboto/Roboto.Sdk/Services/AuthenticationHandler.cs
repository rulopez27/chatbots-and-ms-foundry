using System.Net.Http.Headers;

namespace Roboto.Sdk.Services
{
    /// <summary>
    /// HTTP message handler that adds authentication token to all requests
    /// </summary>
    internal class AuthenticationHandler : DelegatingHandler
    {
        private readonly ITokenStorage _tokenStorage;

        public AuthenticationHandler(ITokenStorage tokenStorage)
        {
            _tokenStorage = tokenStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            // Get the current token
            var token = _tokenStorage.Token;

            // If token exists, add it to the request headers
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
