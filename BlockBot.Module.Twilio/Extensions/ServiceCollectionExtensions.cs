using BlockBot.Module.Twilio.ServiceInterfaces;
using BlockBot.Module.Twilio.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlockBot.Module.Twilio.Extensions
{
    /// <summary>
    ///     Class for extensions to the <see cref="IServiceCollection" />
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTwilioServices(this IServiceCollection services)
        {
            services.AddTransient<ITwilioService, TwilioService>();
            return services;
        }

        public static IServiceCollection AddTwilioIntegrationServices(this IServiceCollection services)
        {
            services.AddTransient<ITwilioService, TwilioService>();
            return services;
        }

    }
}