using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockBot.Common.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlockBot.Web.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly ApplicationRoleManager _applicationRoleManager;
        private readonly ApplicationUserManager _applicationUserManager;

        public ConfigurationController(
            ApplicationRoleManager applicationRoleManager,
             ApplicationUserManager applicationUserManager
            )
        {
            _applicationRoleManager = applicationRoleManager;
            _applicationUserManager = applicationUserManager;
        }

        public async Task<IActionResult> UpsertAdminRole()
        {
            // create role if necessary
            const string roleName = "Admin";
            bool roleExists = await _applicationRoleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                ApplicationRole role = new ApplicationRole(roleName);
                await _applicationRoleManager.CreateAsync(role);
            }

            // grant admin access to Harley Waldstein if his user exists
            ApplicationUser user = await _applicationUserManager.FindByEmailAsync("harley@waldstein.io");
            if (user != null)
            {
                IdentityResult result1 = await _applicationUserManager.AddToRoleAsync(user, roleName);
            }
            
            return new OkResult();
        }


        public async Task<JsonResult> ShowUserClaims()
        {
            ApplicationUser user = await _applicationUserManager.FindByEmailAsync("harley@waldstein.io");
            return new JsonResult(user?.Claims);
        }

        public async Task<IActionResult> CreateIntegrationTemplatesBucket()
        {
            string sourceBucket = "blockbot-integration-templates";
            throw new NotImplementedException();
        }
    }
}