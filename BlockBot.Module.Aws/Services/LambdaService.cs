using System;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime;
using BlockBot.Module.Aws.Models;
using BlockBot.Module.Aws.ServiceInterfaces;
using NLog;

namespace BlockBot.Module.Aws.Services
{
    public class LambdaService : ILambdaService
    {
        private readonly AmazonLambdaClient _client;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public LambdaService(AWSCredentials credentials, RegionEndpoint awsRegion)
        {
            _client = new AmazonLambdaClient(credentials, new AmazonLambdaConfig
            {
                RegionEndpoint = awsRegion
            });
        }

        public async Task<string> CreateLambda(string functionName, string functionDescription, string roleArn, string s3Bucket, string s3Key)
        {
            try
            {
                CreateFunctionResponse result = await _client.CreateFunctionAsync(new CreateFunctionRequest
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
                    Handler = "index.handler",
                    Runtime = Runtime.Nodejs810
                });

                return result.FunctionArn;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public async Task<string> UpdateLambda(string functionName, string s3Bucket, string s3Key)
        {
            try
            {
                UpdateFunctionCodeResponse result = await _client.UpdateFunctionCodeAsync(new UpdateFunctionCodeRequest
                {
                    FunctionName = functionName,
                    S3Bucket = s3Bucket,
                    S3Key = s3Key,
                    Publish = true
                });
                if (result.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Error updating lambda function name '{functionName}'.");
                }

                return result.FunctionArn;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public async Task<bool> DeleteLambda(string functionName)
        {
            try
            {
                DeleteFunctionResponse result = await _client.DeleteFunctionAsync(new DeleteFunctionRequest
                {
                    FunctionName = functionName
                });

                return result.HttpStatusCode == HttpStatusCode.NoContent;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public async Task<LambdaFunction> ReadLamda(string functionName)
        {
            try
            {
                GetFunctionResponse result = await _client.GetFunctionAsync(new GetFunctionRequest
                {
                    FunctionName = functionName
                });

                return new LambdaFunction
                {
                    Configuration = result.Configuration,
                    Code = result.Code,
                    Concurrency = result.Concurrency,
                    Tag = result.Tags
                };
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public async Task<bool> CheckIfLambdaExists(string functionName)
        {
            try
            {
                GetFunctionResponse result = await _client.GetFunctionAsync(new GetFunctionRequest
                {
                    FunctionName = functionName
                });

                return result.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (ResourceNotFoundException)
            {
                return false;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }
    }
}