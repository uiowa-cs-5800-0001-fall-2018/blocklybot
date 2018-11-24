using Microsoft.Extensions.Configuration;

namespace BlockBot.Module.Aws.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="IConfiguration" /> configuration container
    /// </summary>
    public static class ConfigurationExtensions
    {
        public static string GetAwsSecretKey(this IConfiguration configuration)
        {
            return configuration["AwsSecretKey"];
        }

        public static string GetAwsAccessKey(this IConfiguration configuration)
        {
            return configuration["AwsAccessKey"];
        }
    }


}