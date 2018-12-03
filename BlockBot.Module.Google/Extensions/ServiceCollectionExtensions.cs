using BlockBot.Module.Google.Services;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.Extensions.DependencyInjection;

namespace BlockBot.Module.Google.Extensions
{
    /// <summary>
    ///     Class for extensions to the <see cref="IServiceCollection" />
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGoogleServices(this IServiceCollection services)
        {
            services.AddTransient<GoogleCalendarService, GoogleCalendarService>();
            services.AddSingleton<GoogleAccessTokenDataStore, GoogleAccessTokenDataStore>();
            return services;
        }
    }
}