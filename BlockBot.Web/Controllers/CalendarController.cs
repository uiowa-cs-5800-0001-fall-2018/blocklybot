using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockBot.Common.Data;
using BlockBot.Module.Google.Services;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace BlockBot.Web.Controllers
{
    public class CalendarController : Controller
    {
        private readonly GoogleCalendarService _googleCalendarService;
        private readonly ApplicationUserManager _userManager;
        private ApplicationDbContext _context;

        public CalendarController(GoogleCalendarService googleCalendarService, ApplicationUserManager userManager, ApplicationDbContext context)
        {
            this._googleCalendarService = googleCalendarService;
            this._userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var x = _googleCalendarService.ListCalendars(ref _context, user.NormalizedUserName);
            int y = 0;
            return View(x);
        }
    }
}