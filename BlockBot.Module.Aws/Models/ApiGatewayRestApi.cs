using Amazon.APIGateway.Model;

namespace BlockBot.Module.Aws.Models
{
    public class ApiGatewayRestApi
    {
        public CreateRestApiResponse RestApi { get; set; }
        public string RestApiId { get; set; }
    }
}
