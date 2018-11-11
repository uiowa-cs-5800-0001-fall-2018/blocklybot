using System.Linq;
using System.Threading.Tasks;
using BlockBot.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlockBot.Web.Controllers
{
    public class WorkspaceController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationDbContext _context;

        public WorkspaceController(ApplicationDbContext context,
            ApplicationUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The project id</param>
        /// <returns></returns>
        public async Task<IActionResult> Workspace(int? id)
        {
            ViewData["Message"] = "A workspace for editing programs";

            if (id == null)
            {
                return NotFound();
            }

            Project project = await _context.Projects
                .FirstOrDefaultAsync(m => m.ProjectId == id);

            if (project == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (project.OwnerId != user.Id)
            {
                return Unauthorized();
            }

            return View(project);
        }

        [HttpPut]
        public async Task<IActionResult> PutProjectXml(int id, string xml)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound($"Unable to load project with ID '{id}'.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (project.OwnerId != user.Id)
            {
                return Unauthorized();
            }

            project.XML = xml;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> PutProjectCode(int id, string code)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound($"Unable to load project with ID '{id}'.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (project.OwnerId != user.Id)
            {
                return Unauthorized();
            }

            // TODO update lambda function(s)

            return NoContent();
        }
    }
}