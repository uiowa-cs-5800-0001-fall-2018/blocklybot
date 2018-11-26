using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BlockBot.Web.Controllers
{
    [AllowAnonymous]
    public class DiagnosticsController : Controller
    {
        private static readonly string _configurationPrefix = "FundChat_";

        private static readonly string _connectionStringPrefix = "ConnectionStrings";

        private readonly IConfiguration _configuration;

        public DiagnosticsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JsonResult Configuration()
        {
            // TODO consider making this local only or otherwise locking it down

            IEnumerable<string> settingNamesWithoutPrefix = _configuration
                .GetChildren()
                .Where(x => x.Key.StartsWith(_configurationPrefix))
                .Select(x => x.Key.Substring(_configurationPrefix.Length))
                .Where(x => !x.StartsWith(_connectionStringPrefix)); // remove connection string settings

            return Json(_configuration.GetChildren().Where(x => settingNamesWithoutPrefix.Contains(x.Key)));
        }

        public JsonResult ConnectionStrings()
        {
            // TODO consider making this local only or otherwise locking it down
            return Json(_configuration.GetSection(_connectionStringPrefix).GetChildren());
        }
    }
}