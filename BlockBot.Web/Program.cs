using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace BlockBot.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            string configurationPrefix = "FundChat_";

            Dictionary<string, string> configDictionary = FetchContainerConfiguration(configurationPrefix);

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddInMemoryCollection(configDictionary);

                    // import environment variables with the given prefix
                    config.AddEnvironmentVariables(configurationPrefix);
                })
                .UseStartup<Startup>();
        }

        /// <summary>
        ///     Elastic beanstalk does not set instance environment variables. Use this function to fetch the instance environment
        ///     variables from their configuration file.
        /// </summary>
        /// <param name="configurationPrefix">
        ///     A prefix to remove from any configuration key names.
        /// </param>
        /// <returns>
        ///     A dictionary of keys and values, with the the prefix removed from the beginning of any keys if present.
        /// </returns>
        /// <remarks>
        ///     https://stackoverflow.com/a/43221055
        /// </remarks>
        private static Dictionary<string, string> FetchContainerConfiguration(string configurationPrefix)
        {
            IConfigurationBuilder elasticBeanstalkConfigBuilder = new ConfigurationBuilder();

            elasticBeanstalkConfigBuilder.AddJsonFile(
                @"C:\Program Files\Amazon\ElasticBeanstalk\config\containerconfiguration",
                true,
                true
            );

            IConfigurationRoot elasticBeanstalkConfig = elasticBeanstalkConfigBuilder.Build();

            Dictionary<string, string> ebConfig = elasticBeanstalkConfig
                .GetSection("iis:env")
                .GetChildren()
                .Select(pair => pair.Value.Split(new[] {'='}, 2))
                .ToDictionary(keyPair => keyPair[0], keyPair => keyPair[1]);

            Dictionary<string, string> configDictionary = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> keyValuePair in ebConfig)
            {
                configDictionary.Add(
                    keyValuePair.Key.StartsWith(configurationPrefix)
                        ? keyValuePair.Key.Substring(configurationPrefix.Length)
                        : keyValuePair.Key, keyValuePair.Value);
            }

            return configDictionary;
        }
    }
}