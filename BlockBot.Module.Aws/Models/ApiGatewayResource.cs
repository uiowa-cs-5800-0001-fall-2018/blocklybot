using Amazon.APIGateway.Model;

namespace BlockBot.Module.Aws.Models
{
    public class ApiGatewayResource
    {
        public CreateResourceResponse Resource { get; set; }
        public PutMethodResponse Method { get; set; }
        public PutIntegrationResponse Integration { get; set; }
        public string RestApiId { get; set; }
        public string ResourceId { get; set; }
        public string DeploymentId { get; set; }
    }
}
