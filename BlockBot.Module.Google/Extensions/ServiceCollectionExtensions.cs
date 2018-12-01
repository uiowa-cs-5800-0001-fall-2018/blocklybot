using BlockBot.Module.Google.Services;
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
            return services;
        }
    }
}