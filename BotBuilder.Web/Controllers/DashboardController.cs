using System.Linq;
using System.Threading.Tasks;
using BotBuilder.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BotBuilder.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewData["Message"] = "A user's dashboard for seeing their projects, creating new projects, etc.";

            return View(await _context.Projects.ToListAsync());
        }
    }
}