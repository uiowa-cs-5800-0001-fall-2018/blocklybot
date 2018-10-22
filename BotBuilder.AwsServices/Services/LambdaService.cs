using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime;
using BotBuilder.AwsServices.ServiceInterfaces;

namespace BotBuilder.AwsServices.Services
{
    public class LambdaService : ILambdaService
    {
        private readonly AmazonLambdaClient _client;

        public LambdaService(AWSCredentials credentials, RegionEndpoint awsRegion)
        {
            _client = new AmazonLambdaClient(credentials, new AmazonLambdaConfig
            {
                RegionEndpoint = awsRegion
            });
        }

        public void CreateLambda()
        {
            Task<CreateFunctionResponse> a = _client.CreateFunctionAsync(new CreateFunctionRequest
            {
                FunctionName = "",
                Code = new FunctionCode
                {
                    S3Bucket = ""
                }
            });

            _client.CreateEventSourceMappingAsync(new CreateEventSourceMappingRequest
            {
                EventSourceArn = "",
                Enabled = true
            });
        }
    }
}