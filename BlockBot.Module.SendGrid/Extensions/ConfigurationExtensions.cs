using Microsoft.Extensions.Configuration;

namespace BlockBot.Module.SendGrid.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="IConfiguration" /> configuration container
    /// </summary>
    public static class ConfigurationExtensions
    {
        public static string GetSendGridApiKey(this IConfiguration configuration)
        {
            return configuration["SendGridApiKey"];
        }
    }
}