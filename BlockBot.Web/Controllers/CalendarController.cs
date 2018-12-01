using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockBot.Module.Google.Services;
using BlockBot.Web.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Mvc;

namespace BlockBot.Web.Controllers
{
    public class CalendarController : Controller
    {
        private readonly GoogleCalendarService _googleCalendarService;
        private readonly ApplicationUserManager _userManager;

        public CalendarController(GoogleCalendarService googleCalendarService, ApplicationUserManager userManager)
        {
            this._googleCalendarService = googleCalendarService;
            this._userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            CalendarListResource.ListRequest x = _googleCalendarService.ListCalendars(user.NormalizedUserName);
            CalendarList z = x.Execute();
            int y = 0;
            return View(z);
        }
    }
}