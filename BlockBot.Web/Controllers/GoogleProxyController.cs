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
        private readonly GoogleCalendarService _googleCalendarService;

        public GoogleProxyController(
            ApplicationDbContext context,
            GoogleCalendarService googleCalendarService
        )
        {
            _context = context;
            _googleCalendarService = googleCalendarService;
        }

        [HttpPost]
        public JsonResult CreateCalendarEvent(string username, string calendarId, string title, int startYear,
            int startMonth, int startDay, int startHour, int startMinute, int durationInMinutes)
        {
            DateTime startTime = new DateTime(startYear, startMonth, startDay, startHour, startMinute, 0);
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

        [HttpPost]
        public JsonResult GetNextNCalendarEvents(string username, string calendarId, int startYear, int startMonth,
            int startDay, int startHour, int startMinute, int n)
        {
            DateTime startTime = new DateTime(startYear, startMonth, startDay, startHour, startMinute, 0);
            return new JsonResult(_googleCalendarService
                .GetNextNEvents(ref _context, username, calendarId, startTime, n));
        }

        [HttpPost]
        public JsonResult GetNextNAvailableCalendarEventSlots(string username, string calendarId, int startYear, int startMonth,
            int startDay, int startHour, int startMinute, int n, int durationInMinutes)
        {
            DateTime startTime = new DateTime(startYear, startMonth, startDay, startHour, startMinute, 0);
            return new JsonResult(_googleCalendarService
                .GetNextNAvailableCalendarEventSlots(ref _context, username, calendarId, startTime, n, durationInMinutes));
        }
    }
}