using System.Threading.Tasks;
using Amazon.APIGateway.Model;
using BlockBot.Module.Aws.Models;

namespace BlockBot.Module.Aws.ServiceInterfaces
{
    public interface IApiGatewayService
    {
        Task<ApiGatewayRestApi> CreateApiGateway(string apiName, string apiDescription);

        Task<ApiGatewayRestApiDelete> DeleteApiGateway(string restApiId);

        Task<ApiGatewayResource> CreateResourceMappedToLambda(string restApiId, string resourceName, string functionArn, string roleArn);

        Task<ApiGatewayResourceDelete> DeleteResourceMappedToLambda(string restApiId, string resourceId);

        Task<CreateDeploymentResponse> DeployRestApi(string restApiId, string deploymentStageName = "default");
    }
}
