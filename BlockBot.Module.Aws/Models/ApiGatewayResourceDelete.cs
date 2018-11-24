using Amazon.APIGateway.Model;

namespace BlockBot.Module.Aws.Models
{
    public class ApiGatewayResourceDelete
    {
        public DeleteIntegrationResponse Integration { get; set; }
        public DeleteMethodResponse Method { get; set; }
        public DeleteResourceResponse Resource { get; set; }
    }
}
