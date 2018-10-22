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