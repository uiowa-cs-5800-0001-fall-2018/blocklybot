using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlockBot.Common.Data;
using BlockBot.Common.Functional;
using BlockBot.Module.Google.Extensions;
using BlockBot.Module.Google.Models;
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

        public Either<CalendarList, ReauthorizationRequest> ListCalendars(ref ApplicationDbContext _context, string username)
        {
            Either<CalendarList, ReauthorizationException> x = null;
            try
            {
                InitializeIfNecessary(ref _context,username);
                return _service.CalendarList.List().ExecuteAsync().Result;
            }
            catch (Exception e)
            {
               // likely need to get new oauth refresh token
                return new Either<CalendarList, ReauthorizationRequest>(new ReauthorizationRequest());
            }
        }

        public Event CreateEvent(ref ApplicationDbContext _context, string username, string calendarId, Event eventObject)
        {
            InitializeIfNecessary(ref _context, username);
            return _service.Events.Insert(eventObject, calendarId).Execute();
        }

        public IList<Event> GetNextNEvents(ref ApplicationDbContext _context, string username, string calendarId, DateTime now, int n)
        {
            InitializeIfNecessary(ref _context, username);
            var x = _service.Events.List(calendarId);
            x.TimeMin = now;
            x.MaxResults = n;
            return x.Execute().Items;
        }

        public DateTime GetNextNAvailableCalendarEventSlots(ref ApplicationDbContext _context, string username, string calendarId, DateTime now, int n, int durationInMinutes)
        {
            InitializeIfNecessary(ref _context, username);
            var listRequest = _service.Events.List(calendarId);
            listRequest.TimeMin = now;
            var events = listRequest.Execute().Items;
            var list = events.OrderBy(x => x.Start.DateTime).ToList();
            if (list.Count == 0)
            {
                return now;
            }
            if (list[0].Start.DateTime > now.AddMinutes(durationInMinutes))
            {
                return now;
            }
            for (int i = 1; i < list.Count; i++)
            {
                if (list[i].Start.DateTime > list[i-1].End.DateTime.Value.AddMinutes(durationInMinutes))
                {
                    return list[i - 1].End.DateTime.Value;
                }
            }

            return list[list.Count - 1].End.DateTime.Value;
        }

    }
}