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

            IConfigurationRoot ebsConfig = FetchElasticBeanstalkConfiguration(configurationPrefix);

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddConfiguration(ebsConfig);

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
        private static IConfigurationRoot FetchElasticBeanstalkConfiguration(string configurationPrefix)
        {
            string connectionStringPrefix = "ConnectionStrings:";
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
                .ToDictionary(keyPair => keyPair[0].Replace("__", ":"), keyPair => keyPair[1]);

            Dictionary<string, string> configDictionary = new Dictionary<string, string>();
            Dictionary<string, string> connectionStringDictionary = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> keyValuePair in ebConfig)
            {
                string key = keyValuePair.Key;

                if (key.StartsWith(configurationPrefix))
                {
                    key = key.Substring(configurationPrefix.Length);
                }

                if (key.StartsWith(connectionStringPrefix))
                {
                    key = key.Substring(connectionStringPrefix.Length);

                    connectionStringDictionary.Add(key, keyValuePair.Value);
                }
                else
                {
                    configDictionary.Add(key, keyValuePair.Value);
                }
            }

            IConfigurationBuilder outBuilder = new ConfigurationBuilder();
            outBuilder.AddInMemoryCollection(configDictionary);

            IConfigurationRoot configurationRoot = outBuilder.Build();

            foreach (KeyValuePair<string, string> kvp in connectionStringDictionary)
            {
                configurationRoot.GetSection("ConnectionStrings")[kvp.Key] = kvp.Value;
            }

            return configurationRoot;
        }
    }
}