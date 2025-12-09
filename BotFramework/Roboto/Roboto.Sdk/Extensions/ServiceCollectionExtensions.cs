using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Roboto.Sdk.Configuration;
using Roboto.Sdk.Services;

namespace Roboto.Sdk.Extensions
{
    /// <summary>
    /// Extension methods for IServiceCollection to register Roboto SDK services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Roboto API client services to the service collection
        /// </summary>
        public static IServiceCollection AddRobotoApiClient(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Bind configuration
            services.Configure<RobotoApiOptions>(
                configuration.GetSection(RobotoApiOptions.SectionName));

            var options = configuration
                .GetSection(RobotoApiOptions.SectionName)
                .Get<RobotoApiOptions>();

            if (options == null || string.IsNullOrEmpty(options.BaseUrl))
            {
                throw new InvalidOperationException(
                    $"RobotoApi:BaseUrl configuration is required. " +
                    $"Add a '{RobotoApiOptions.SectionName}' section to your appsettings.json");
            }

            // Register HttpClient for each service
            services.AddHttpClient<IAuthenticationService, AuthenticationService>(client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            });

            services.AddHttpClient<IUserService, UserService>(client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            });

            services.AddHttpClient<ICalendarEventService, CalendarEventService>(client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            });

            // Register the main client
            services.AddScoped<RobotoApiClient>();

            return services;
        }

        /// <summary>
        /// Add Roboto API client services with custom configuration
        /// </summary>
        public static IServiceCollection AddRobotoApiClient(
            this IServiceCollection services,
            Action<RobotoApiOptions> configureOptions)
        {
            var options = new RobotoApiOptions();
            configureOptions(options);

            services.Configure(configureOptions);

            return services.AddRobotoApiClient(options);
        }

        private static IServiceCollection AddRobotoApiClient(
            this IServiceCollection services,
            RobotoApiOptions options)
        {
            if (string.IsNullOrEmpty(options.BaseUrl))
            {
                throw new ArgumentException("BaseUrl is required", nameof(options));
            }

            // Register HttpClient for each service
            services.AddHttpClient<IAuthenticationService, AuthenticationService>(client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            });

            services.AddHttpClient<IUserService, UserService>(client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            });

            services.AddHttpClient<ICalendarEventService, CalendarEventService>(client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            });

            // Register the main client
            services.AddScoped<RobotoApiClient>();

            return services;
        }
    }
}
