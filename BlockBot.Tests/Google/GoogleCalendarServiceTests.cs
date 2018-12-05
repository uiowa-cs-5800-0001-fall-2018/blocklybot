using System;
using System.Collections.Generic;
using BlockBot.Module.Google.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlockBot.Tests.Google
{
    [TestClass]
    public class GoogleCalendarServiceTests
    {
        private static GoogleCalendarService _service;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {

        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestMethod]
        public void ListCalendar_ShouldHaveAtLeastPersonalCalendar()
        {

            //_service.ListCalendars("HARLEY@WALDSTEIN.IO");
        }
    }
}