using System.Threading.Tasks;
using BlockBot.Module.Aws.Models;

namespace BlockBot.Module.Aws.ServiceInterfaces
{
    public interface ILambdaService
    {
        Task<string> CreateLambda(string functionName, string functionDescription, string roleArn,
            string s3Bucket, string s3Key);

        Task<string> UpdateLambda(string functionName, string s3Bucket, string s3Key);

        Task<bool> DeleteLambda(string functionName);

        Task<LambdaFunction> ReadLamda(string functionName);

        Task<bool> CheckIfLambdaExists(string functionName);
    }
}