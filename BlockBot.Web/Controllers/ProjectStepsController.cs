using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockBot.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlockBot.Web.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectStepsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectStepsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ProjectSteps
        [HttpGet]
        public IEnumerable<ProjectStep> GetProjectSteps()
        {
            return _context.ProjectSteps;
        }

        // GET: api/ProjectSteps/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectStep([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projectStep = await _context.ProjectSteps.FindAsync(id);

            if (projectStep == null)
            {
                return NotFound();
            }

            return Ok(projectStep);
        }

        // PUT: api/ProjectSteps/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectStep([FromRoute] int id, [FromBody] ProjectStep projectStep)
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

        // POST: api/ProjectSteps
        [HttpPost]
        public async Task<IActionResult> PostProjectStep([FromBody] ProjectStep projectStep)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProjectSteps.Add(projectStep);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProjectStep", new { id = projectStep.ProjectStepId }, projectStep);
        }

        // DELETE: api/ProjectSteps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectStep([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projectStep = await _context.ProjectSteps.FindAsync(id);
            if (projectStep == null)
            {
                return NotFound();
            }

            _context.ProjectSteps.Remove(projectStep);
            await _context.SaveChangesAsync();

            return Ok(projectStep);
        }

        private bool ProjectStepExists(int id)
        {
            return _context.ProjectSteps.Any(e => e.ProjectStepId == id);
        }
    }
}