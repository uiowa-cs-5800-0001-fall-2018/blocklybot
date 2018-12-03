using System;
using BlockBot.Common.Data;
using BlockBot.Module.Google.Services;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlockBot.Web.Controllers
{
    [AllowAnonymous]
    public class GoogleProxyController : Controller
    {
        private ApplicationDbContext _context;
        private GoogleCalendarService _googleCalendarService;

        public GoogleProxyController(
            ApplicationDbContext context,
            GoogleCalendarService googleCalendarService
        )
        {
            _context = context;
            _googleCalendarService = googleCalendarService;
        }

        [HttpPost]
        public JsonResult CreateCalendarEvent(string username, string calendarId, string title, int startYear, int startMonth, int startDay, int startHour, int startMinute, int durationInMinutes)
        {
            var startTime = new DateTime(startYear, startMonth, startDay, startHour, startMinute, 0);
            return new JsonResult(_googleCalendarService
                .CreateEvent(ref _context,
                    username,
                    calendarId,
                    new Event
                    {
                        Summary = title,
                        Start = new EventDateTime
                        {
                            DateTime = startTime
                        },
                        End = new EventDateTime
                        {
                            DateTime = startTime.AddMinutes(durationInMinutes)
                        }
                    }));
        }
    }
}