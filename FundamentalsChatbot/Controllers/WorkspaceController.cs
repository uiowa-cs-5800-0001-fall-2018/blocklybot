using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundamentalsChatbot.Controllers
{
    public class WorkspaceController : Controller
    {
        public IActionResult Workspace()
        {
            ViewData["Message"] = "A workspace for editing programs";

            return View();
        }
    }
}