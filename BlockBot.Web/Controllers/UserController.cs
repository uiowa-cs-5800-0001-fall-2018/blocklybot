using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockBot.Common.Data;
using BlockBot.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace BlockBot.Web.Controllers
{
    public class UserController : Controller
    {
        private ApplicationDbContext _context;
        private ApplicationUserManager _userManager;

        public UserController(ApplicationDbContext context, ApplicationUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Projects(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            var model = new User_Projects_Model
            {
                User = user,
                Projects = _context.Projects.Where(x => x.OwnerId == user.Id && x.IsPublic).ToList()
            };

            return View(model);
        }
    }
}