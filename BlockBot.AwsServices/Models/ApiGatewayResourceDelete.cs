using System;
using System.Collections.Generic;
using System.Text;
using Amazon.APIGateway.Model;

namespace BlockBot.AwsServices.Models
{
    public class ApiGatewayResourceDelete
    {
        public DeleteIntegrationResponse Integration { get; set; }
        public DeleteMethodResponse Method { get; set; }
        public DeleteResourceResponse Resource { get; set; }
    }
}
