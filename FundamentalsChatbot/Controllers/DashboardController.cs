using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundamentalsChatbot.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewData["Message"] = "A user's dashboard for seeing their projects, creating new projects, etc.";

            return View();
        }
    }
}