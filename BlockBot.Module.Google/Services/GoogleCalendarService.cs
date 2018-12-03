using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlockBot.Common.Data;
using BlockBot.Module.Google.Extensions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlockBot.Module.Google.Services
{
    public class GoogleCalendarService
    {
        private static readonly string[] Scopes =
            {CalendarService.Scope.CalendarReadonly, CalendarService.Scope.CalendarEvents};

        private static readonly string ApplicationName = "BlockBot";
        private readonly ClientSecrets _clientSecrets;

        private CalendarService _service;


        public GoogleCalendarService(IConfiguration configuration)
        {
            _clientSecrets = new ClientSecrets
            {
                ClientId = configuration.GetGoogleClientId(),
                ClientSecret = configuration.GetGoogleClientSecret()
            };
        }

        private void InitializeIfNecessary(ref ApplicationDbContext _context, string username)
        {
            if (_service == null)
            {
                GoogleAuthorizationCodeFlow _flow = new GoogleAuthorizationCodeFlow(
                    new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = _clientSecrets,
                        Scopes = Scopes,
                        DataStore = new NullDataStore()
                    });

                TokenResponse x = _context.GetAsync<TokenResponse>(username);

                if (x == null || x.IsExpired(_flow.Clock))
                {
                    // TODO log that we're refreshing the google access token
                    string refreshToken = _context.GetRefreshToken(username);
                    if (refreshToken == null)
                    {
                        throw new Exception(
                            $"Cannot initialize GoogleCalendarService for user '{username}', no refresh token found.");
                    }

                    x = _flow.RefreshTokenAsync(username, refreshToken, CancellationToken.None).Result;
                    _context.StoreAsync(username, x);
                }

                // Create Google Calendar API service.
                _service = new CalendarService(new BaseClientService.Initializer
                {
                    ApplicationName = ApplicationName,
                    HttpClientInitializer = new UserCredential(_flow, username, x)
                });
            }
        }

        public CalendarList ListCalendars(ref ApplicationDbContext _context, string username)
        {
            InitializeIfNecessary(ref _context,username);
            return _service.CalendarList.List().ExecuteAsync().Result;
        }
        public Event CreateEvent(ref ApplicationDbContext _context, string username, string calendarId, Event eventObject)
        {
            InitializeIfNecessary(ref _context, username);
            return _service.Events.Insert(eventObject, calendarId).Execute();
        }

    }
}