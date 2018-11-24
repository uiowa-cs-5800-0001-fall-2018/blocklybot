using System.Threading.Tasks;
using BlockBot.Module.SendGrid.Extensions;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BlockBot.Module.SendGrid.Services
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-2.1&tabs=visual-studio#require-email-confirmation
    /// </summary>
    public class SendGridEmailSender : IEmailSender
    {
        private readonly string _sendGridApiKey;

        public SendGridEmailSender(IConfiguration configuration)
        {
            this._sendGridApiKey = configuration.GetSendGridApiKey();
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(this._sendGridApiKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("admin@blockbot.io"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}
