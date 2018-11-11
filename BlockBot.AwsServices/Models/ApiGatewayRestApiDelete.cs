using System;
using System.Collections.Generic;
using System.Text;
using Amazon.APIGateway.Model;

namespace BlockBot.AwsServices.Models
{
    public class ApiGatewayRestApiDelete
    {
        public DeleteRestApiResponse RestApi { get; set; }
    }
}
