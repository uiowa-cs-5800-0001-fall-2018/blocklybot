using BlockBot.module.Integrations.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlockBot.module.Integrations.Extensions
{
    /// <summary>
    ///     Class for extensions to the <see cref="IServiceCollection" />
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIntegrationsServices(this IServiceCollection services)
        {
            services.AddTransient<IntegrationCreationService, IntegrationCreationService>();
            return services;
        }
    }
}