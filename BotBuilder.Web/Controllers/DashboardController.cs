using BotBuilder.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace BotBuilder.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            ViewData["Message"] = "A user's dashboard for seeing their projects, creating new projects, etc.";

            return View();
        }
    }
}