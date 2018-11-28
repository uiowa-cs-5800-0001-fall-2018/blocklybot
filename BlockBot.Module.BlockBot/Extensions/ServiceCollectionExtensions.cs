using BlockBot.Module.BlockBot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlockBot.Module.BlockBot.Extensions
{
    /// <summary>
    ///     Class for extensions to the <see cref="IServiceCollection" />
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlockBotIntegrationServices(this IServiceCollection services)
        {
            services.AddTransient<BlockBotIntegrationCreationService, BlockBotIntegrationCreationService>();
            return services;
        }
    }
}