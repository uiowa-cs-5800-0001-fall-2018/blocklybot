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

        public void CreateLambda(string functionName, string functionDescription, string s3Bucket, string s3Key, string roleArn)
        {
            CreateFunctionResponse result = _client.CreateFunctionAsync(new CreateFunctionRequest
            {
                FunctionName = functionName,
                Code = new FunctionCode
                {
                    S3Bucket = s3Bucket,
                    S3Key = s3Key
                },
                Publish = true,
                Role = roleArn,
                Description = functionDescription,
                Handler = "index.handler"
            }).Result;
        }

        public void UpdateLambda(string functionName, string s3Bucket, string s3Key)
        {
            UpdateFunctionCodeResponse result = _client.UpdateFunctionCodeAsync(new UpdateFunctionCodeRequest
            {
                FunctionName = functionName,
                S3Bucket = s3Bucket,
                S3Key = s3Key,
                Publish = true
            }).Result;
        }
    }
}