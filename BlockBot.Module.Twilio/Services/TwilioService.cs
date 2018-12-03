using System;
using BlockBot.Module.Twilio.Extensions;
using BlockBot.Module.Twilio.ServiceInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Lookups.V1;
using Twilio.Rest.Messaging.V1;
using PhoneNumberResource = Twilio.Rest.Messaging.V1.Service.PhoneNumberResource;

namespace BlockBot.Module.Twilio.Services
{
    public class TwilioService : ITwilioService
    {
        private readonly ILogger<TwilioService> _logger;
        private readonly string _twilioAccountSid;
        private readonly string _twilioAuthToken;

        public TwilioService(IConfiguration configuration, ILogger<TwilioService> logger)
        {
            _twilioAccountSid = configuration.GetTwilioAccountSid();
            _twilioAuthToken = configuration.GetTwilioAuthToken();
            _logger = logger;
        }

        public void UpdateServiceProcessingUrl(string url, string serviceSid)
        {
            try
            {
                TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);

                ServiceResource.Update(new UpdateServiceOptions(serviceSid)
                {
                    FriendlyName = "Appointment Messaging Service",
                    InboundRequestUrl = new Uri(url),
                    
                });
            }
            catch (Exception e)
            {
                // TODO add logger via dependency injection
                _logger.LogError(e,
                    $"Error updating twilio service sid '{serviceSid}' to have inbound request url '{url}' on account sid '{_twilioAccountSid}'.");
                throw;
            }
        }
    }
}