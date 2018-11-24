using BlockBot.Module.SendGrid.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlockBot.Module.SendGrid.Extensions
{
    /// <summary>
    ///     Class for extensions to the <see cref="IServiceCollection"/>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSendGridServices(this IServiceCollection services)
        {
            services.AddSingleton<IEmailSender, SendGridEmailSender>();
            return services;
        }
    }
}
