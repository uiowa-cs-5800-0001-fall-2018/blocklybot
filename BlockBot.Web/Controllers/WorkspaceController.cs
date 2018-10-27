using System.Linq;
using System.Threading.Tasks;
using BlockBot.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlockBot.Web.Controllers
{
    public class WorkspaceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkspaceController(ApplicationDbContext context)
        {
            _context = context;
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

            return View(project);
        }

        [HttpPut]
        public async Task<IActionResult> PutProjectStep(int id, ProjectStep projectStep)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != projectStep.ProjectStepId)
            {
                return BadRequest();
            }

            _context.Entry(projectStep).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectStepExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> PutProjectStepCode(int id, string code)
        {
            // TODO update lambda function(s)

            return NoContent();
        }

        private bool ProjectStepExists(int id)
        {
            return _context.ProjectSteps.Any(e => e.ProjectStepId == id);
        }
    }
}