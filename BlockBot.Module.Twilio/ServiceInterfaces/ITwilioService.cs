namespace BlockBot.Module.Twilio.ServiceInterfaces
{
    public interface ITwilioService
    {
        void UpdateServiceProcessingUrl(string url, string serviceSid, string accountSid, string accountAuthToken);
    }
}
