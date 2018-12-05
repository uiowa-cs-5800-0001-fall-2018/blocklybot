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
using BlockBot.Module.Aws.ServiceInterfaces;
using BlockBot.Module.Aws.Services;
using BlockBot.Module.Twilio.ServiceInterfaces;

namespace BlockBot.Module.Twilio.Services
{
    public class TwilioIntegrationCreationService : IIntegrationCreationService
    {
        /// <summary>
        /// Name of service, in lower case
        /// </summary>
        /// <returns></returns>
        public static string ServiceName() => "twilio";

        private readonly IApiGatewayService _apiGatewayService;
        private readonly ILambdaService _lambdaService;
        private readonly IS3Service _s3Service;
        private readonly ITwilioService _twilioService;

        public TwilioIntegrationCreationService(
            IApiGatewayService apiGatewayService,
            ILambdaService lambdaService,
            IS3Service s3Service,
            ITwilioService twilioService)
        {
            _apiGatewayService = apiGatewayService;
            _lambdaService = lambdaService;
            _s3Service = s3Service;
            _twilioService = twilioService;
        }

        public async Task Integrate(string serviceSid, string accountSid, string authToken, string newApi)
        {
            _twilioService.UpdateServiceProcessingUrl(newApi, serviceSid, accountSid, authToken);
        }
    }
}