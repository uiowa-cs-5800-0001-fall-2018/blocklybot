using System.Threading.Tasks;
using BlockBot.Module.Aws.Models;
using BlockBot.Module.Aws.ServiceInterfaces;

namespace BlockBot.Module.Aws.ServiceMocks
{
    internal class MockLambdaService : ILambdaService
    {
        public Task<string> CreateLambda(string functionName, string functionDescription, string roleArn, string s3Bucket, string s3Key)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> UpdateLambda(string functionName, string s3Bucket, string s3Key)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteLambda(string functionName)
        {
            throw new System.NotImplementedException();
        }

        public Task<LambdaFunction> ReadLamda(string functionName)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> CheckIfLambdaExists(string functionName)
        {
            throw new System.NotImplementedException();
        }
    }
}