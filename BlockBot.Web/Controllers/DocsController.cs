﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BlockBot.Web.Controllers
{
    public class DocsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateTwilioAccount()
        {
            return View();
        }
    }
}