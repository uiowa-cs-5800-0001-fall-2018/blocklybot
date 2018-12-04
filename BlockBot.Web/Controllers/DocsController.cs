using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlockBot.Web.Controllers
{
    [AllowAnonymous]
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

        public IActionResult Components()
        {
            return View();
        }

        public IActionResult EndConversation()
        {
            return View();
        }

        public IActionResult Prompts()
        {
            return View();
        }

        public IActionResult StartConversation()
        {
            return View();
        }
    }
}