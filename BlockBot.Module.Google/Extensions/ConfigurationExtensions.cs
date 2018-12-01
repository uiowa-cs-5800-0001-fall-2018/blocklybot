using Microsoft.Extensions.Configuration;

namespace BlockBot.Module.Google.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="IConfiguration" /> configuration container
    /// </summary>
    public static class ConfigurationExtensions
    {
        public static string GetGoogleClientId(this IConfiguration configuration)
        {
            return configuration["GoogleClientId"];
        }

        public static string GetGoogleClientSecret(this IConfiguration configuration)
        {
            return configuration["GoogleClientSecret"];
        }
    }


}