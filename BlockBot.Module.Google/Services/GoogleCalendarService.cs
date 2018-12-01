using System.Threading;
using BlockBot.Module.Google.Extensions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;

namespace BlockBot.Module.Google.Services
{
    public class GoogleCalendarService
    {
        private static readonly string[] Scopes =
            {CalendarService.Scope.CalendarReadonly, CalendarService.Scope.CalendarEvents};

        private static readonly string ApplicationName = "BlockBot";

        private readonly ClientSecrets _clientSecrets;
        private UserCredential _credential;
        private CalendarService _service;

        public GoogleCalendarService(IConfiguration configuration)
        {
            _clientSecrets = new ClientSecrets
            {
                ClientId = configuration.GetGoogleClientId(),
                ClientSecret = configuration.GetGoogleClientSecret()
            };
        }

        private void InitializeIfNecessary(string username)
        {
            if (_service == null)
            {
                _credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    _clientSecrets,
                    Scopes,
                    username,
                    CancellationToken.None).Result;

                var x = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer()
                {
                    ClientSecrets = _clientSecrets,
                    Scopes = Scopes,
                    DataStore = 

                });

                x.ExchangeCodeForTokenAsync()

                // Create Google Calendar API service.
                _service = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = _credential,
                    ApplicationName = ApplicationName
                });
            }
        }

        public CalendarListResource.ListRequest ListCalendars(string username)
        {
            InitializeIfNecessary(username);

            return _service.CalendarList.List();
        }
    }
}