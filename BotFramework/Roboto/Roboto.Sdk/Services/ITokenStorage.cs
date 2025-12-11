namespace Roboto.Sdk.Services
{
    /// <summary>
    /// Service for storing and retrieving the authentication token within a request scope
    /// </summary>
    public interface ITokenStorage
    {
        string? Token { get; set; }
    }

    internal class TokenStorage : ITokenStorage
    {
        public string? Token { get; set; }
    }
}
