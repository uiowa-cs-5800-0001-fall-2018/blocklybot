using System;
using System.Collections.Generic;
using System.Text;
using Amazon.APIGateway.Model;

namespace BlockBot.AwsServices.Models
{
    public class ApiGatewayRestApi
    {
        public CreateRestApiResponse RestApi { get; set; }
        public string RestApiId { get; set; }
    }
}
