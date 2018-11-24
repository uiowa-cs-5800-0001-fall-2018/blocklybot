using Microsoft.Extensions.Configuration;

namespace BlockBot.Module.Twilio.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="IConfiguration" /> configuration container
    /// </summary>
    public static class ConfigurationExtensions
    {
        public static string GetTwilioAccountSid(this IConfiguration configuration)
        {
            return configuration["TwilioAccountSid"];
        }

        public static string GetTwilioAuthToken(this IConfiguration configuration)
        {
            return configuration["TwilioAuthToken"];
        }
    }
}