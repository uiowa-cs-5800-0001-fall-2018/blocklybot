using System;
using System.Collections.Generic;
using System.Text;
using Amazon.APIGateway.Model;

namespace BlockBot.AwsServices.Models
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
