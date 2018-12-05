using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockBot.Common.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlockBot.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace BlockBot.Web.Controllers
{
    [Authorize(Roles="Admin")]
    public class ProjectSettingTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectSettingTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProjectSettingTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProjectSettingType.ToListAsync());
        }

        // GET: ProjectSettingTypes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectSettingType = await _context.ProjectSettingType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectSettingType == null)
            {
                return NotFound();
            }

            return View(projectSettingType);
        }

        // GET: ProjectSettingTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProjectSettingTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,AllowsMany")] ProjectSettingType projectSettingType)
        {
            if (ModelState.IsValid)
            {
                projectSettingType.Id = Guid.NewGuid();
                _context.Add(projectSettingType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(projectSettingType);
        }

        // GET: ProjectSettingTypes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectSettingType = await _context.ProjectSettingType.FindAsync(id);
            if (projectSettingType == null)
            {
                return NotFound();
            }
            return View(projectSettingType);
        }

        // POST: ProjectSettingTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,AllowsMany")] ProjectSettingType projectSettingType)
        {
            if (id != projectSettingType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectSettingType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectSettingTypeExists(projectSettingType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(projectSettingType);
        }

        // GET: ProjectSettingTypes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectSettingType = await _context.ProjectSettingType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectSettingType == null)
            {
                return NotFound();
            }

            return View(projectSettingType);
        }

        // POST: ProjectSettingTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var projectSettingType = await _context.ProjectSettingType.FindAsync(id);
            _context.ProjectSettingType.Remove(projectSettingType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectSettingTypeExists(Guid id)
        {
            return _context.ProjectSettingType.Any(e => e.Id == id);
        }
    }
}
