using Amazon;
using Amazon.Runtime;
using BlockBot.Module.Aws.ServiceInterfaces;
using BlockBot.Module.Aws.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlockBot.Module.Aws.Extensions
{
    /// <summary>
    ///     Class for extensions to the <see cref="IServiceCollection"/>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAwsServices(this IServiceCollection services)
        {
            services.AddTransient<IIamService, IamService>();
            services.AddTransient<IApiGatewayService, ApiGatewayService>();
            services.AddTransient<ILambdaService, LambdaService>();
            services.AddTransient<IS3Service, S3Service>();
            services.AddTransient<DynamoDbService, DynamoDbService>();
            return services;
        }

        public static IServiceCollection AddAwsCredentials(this IServiceCollection services, IConfiguration configuration)
        {
            AWSCredentials awsCredentials = new BasicAWSCredentials(
                configuration.GetAwsAccessKey(), 
                configuration.GetAwsSecretKey());

            services.AddSingleton(awsCredentials);
            return services;
        }

        public static IServiceCollection AddAwsRegion(this IServiceCollection services)
        {
            services.AddSingleton(RegionEndpoint.USEast1);
            return services;
        }
    }
}
