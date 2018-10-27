using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using BotBuilder.AwsServices.ServiceInterfaces;
using NLog;

namespace BotBuilder.AwsServices.Services
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

        public async void UpdateLambda(string functionName, string s3Bucket, string s3Key)
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
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
            
        }

        public async void DeleteLambda()
        {
            try
            {

            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public async void ReadLamda()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public async void CheckIfLamdaExists(string functionName)
        {
            try
            {
                var url = await _client.GetFunctionAsync(new GetFunctionRequest
                {
                    FunctionName = functionName
                });


            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }
    }
}