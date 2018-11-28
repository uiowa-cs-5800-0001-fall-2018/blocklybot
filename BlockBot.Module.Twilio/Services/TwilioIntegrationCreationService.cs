using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.APIGateway.Model;
using Amazon.S3;
using BlockBot.Common.ServiceInterfaces;
using BlockBot.Module.Aws.Models;
using BlockBot.Module.Aws.Services;

namespace BlockBot.Module.Twilio.Services
{
    public class TwilioIntegrationCreationService : IIntegrationCreationService
    {
        /// <summary>
        /// Name of service, in lower case
        /// </summary>
        /// <returns></returns>
        public static string ServiceName() => "twilio";

        private readonly ApiGatewayService _apiGatewayService;
        private readonly LambdaService _lambdaService;
        private readonly S3Service _s3Service;
        private readonly TwilioService _twilioService;

        public TwilioIntegrationCreationService(
            ApiGatewayService apiGatewayService,
            LambdaService lambdaService,
            S3Service s3Service,
            TwilioService twilioService)
        {
            _apiGatewayService = apiGatewayService;
            _lambdaService = lambdaService;
            _s3Service = s3Service;
            _twilioService = twilioService;
        }

        public async Task Integrate(string newApi)
        {
            _twilioService.UpdateServiceProcessingUrl(newApi, "MG2ae64c23b7ba10eb5aecff49998e5ec9");
        }
    }
}