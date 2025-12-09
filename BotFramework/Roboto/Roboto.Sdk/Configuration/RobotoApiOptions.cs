namespace Roboto.Sdk.Configuration
{
    /// <summary>
    /// Configuration options for the Roboto API client
    /// </summary>
    public class RobotoApiOptions
    {
        public const string SectionName = "RobotoApi";

        /// <summary>
        /// Base URL of the Roboto API (e.g., https://api.roboto.com)
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// API timeout in seconds (default: 30)
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Enable retry policy (default: true)
        /// </summary>
        public bool EnableRetry { get; set; } = true;

        /// <summary>
        /// Maximum number of retry attempts (default: 3)
        /// </summary>
        public int MaxRetryAttempts { get; set; } = 3;
    }
}
