using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Amazon.S3;
using BlockBot.Common.Data;
using BlockBot.Module.Aws.Models;
using BlockBot.Module.Aws.ServiceInterfaces;
using BlockBot.Module.Aws.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BlockBot.Web.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ApplicationUserManager _userManager;
        private readonly IApiGatewayService _apiGatewayService;
        private readonly ILambdaService _lambdaService;
        private readonly IS3Service _s3Service;
        private DynamoDbService _dynamoDbService;

        public ProjectsController(ApplicationDbContext context,
            ApplicationUserManager userManager,
            IApiGatewayService apiGatewayService,
            ILambdaService lambdaService,
            IS3Service s3Service,
            DynamoDbService dynamoDbService){
            _context = context;
            _userManager = userManager;
            _apiGatewayService = apiGatewayService;
            _lambdaService = lambdaService;
            _s3Service = s3Service;
            _dynamoDbService = dynamoDbService;
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
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

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,IsPublic,Description,TwilioServiceSID,TwilioAccountSID,TwilioAuthToken")] Project project)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                project.OwnerId = user.Id;
                project.XML =
                    "<xml xmlns=\"http://www.w3.org/1999/xhtml\" id=\"workspaceBlocks\" style=\"display: none;\"></xml>";
                project.Id = Guid.NewGuid();

                // TODO create IAM user for project?

                // create API Gateway
                // TODO convert this to use prod url after sufficient debugging
                ApiGatewayRestApi result = await _apiGatewayService.CreateApiGateway(
                    project.Id.ToString(),
                    "https://localhost:44305/Projects/Details/" + project.Id);
                project.RestApiId = result.RestApiId;
                _context.Add(project);

                // create s3 bucket
                bool bucketCreateSucceeded = await _s3Service.CreateOrUpdateBucket(project.S3BucketName(), S3CannedACL.Private);
                if (!bucketCreateSucceeded)
                {
                    string message = "Unable to create an S3 bucket.";
                    // TODO log message to ILogger
                    return RedirectToAction("Error", "Home", new { message });
                }

                // TODO create DynamoDB table
                await _dynamoDbService.Create(project.Id);

                // add Twilio integration
                Service twilioService = await _context.Services.FirstOrDefaultAsync(x => x.Name == "Twilio");
                if (twilioService == null)
                {
                    string message = "Unable to locate the Twilio service.";
                    // TODO log message to ILogger
                    return RedirectToAction("Error", "Home", new { message });
                }
                _context.Integrations.Add(new Integration
                {
                    Id = Guid.NewGuid(),
                    ProjectId = project.Id,
                    ServiceId = twilioService.Id
                });

                // save changes
                await _context.SaveChangesAsync();
                
                return RedirectToAction("Dashboard", "Dashboard");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            
            ApplicationUser user = await _userManager.GetUserAsync(User);
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

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Description,IsPublic,TwilioServiceSID,TwilioAccountSID,TwilioAuthToken")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Project originalProject = await _context.Projects.FindAsync(id);
            if (originalProject == null)
            {
                return NotFound();
            }
            if (originalProject.OwnerId != user.Id)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    originalProject.Name = project.Name;
                    originalProject.Description = project.Description;
                    originalProject.IsPublic = project.IsPublic;
                    originalProject.TwilioAccountSID = project.TwilioAccountSID;
                    originalProject.TwilioServiceSID = project.TwilioServiceSID;
                    originalProject.TwilioAuthToken = project.TwilioAuthToken;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Dashboard", "Dashboard");
            }

            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
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

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (project.OwnerId != user.Id)
            {
                return Unauthorized();
            }

            try
            {
                // delete API gateway
                ApiGatewayRestApiDelete y = await _apiGatewayService.DeleteApiGateway(project.RestApiId);
                // TODO throw error on failure

            }
            catch (Exception)
            {
                // do nothing
            }
            

            // delete lambdas
            foreach (Integration integration in _context.Integrations.Where(x => x.ProjectId == id))
            {
                bool result = await _lambdaService.DeleteLambda(integration.FunctionName());
                // TODO throw error on failure

                if (integration.Service.Name == "Twilio")
                {
                    // TODO delete Twilio resources
                    // TODO throw error on failure
                }

                _context.Integrations.Remove(integration);
            }

            // delete S3 bucket
            var g = await _s3Service.DeleteObject(project.S3BucketName(), project.Id.ToString() + "-twilio.zip");
            var z = await _s3Service.DeleteBucket(project.S3BucketName());
            // TODO throw error on failure

            // delete DynamoDB table
            await _dynamoDbService.Delete(project.Id);

            // delete IAM user?

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard", "Dashboard");
        }

        private bool ProjectExists(Guid id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }

        // GET: Projects/Create
        public async Task<IActionResult> Clone(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            if (!project.IsPublic)
            {
                return Unauthorized();
            }
            Project tempDtoProject = new Project
            {
                Id = id,
                Name = project.Name,
                Description = project.Description
            };
            

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clone([Bind("Id,Name,IsPublic,Description,TwilioServiceSID,TwilioAccountSID,TwilioAuthToken")] Project project)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                Project oldProject = _context.Projects.Find(project.Id);
                if (oldProject == null)
                {
                    return NotFound($"Unable to load project with ID '{project.Id}'.");
                }
                Project newProject = new Project
                {
                    Id = Guid.NewGuid(),
                    Name = project.Name,
                    Description = project.Description,
                    IsPublic = project.IsPublic,
                    TwilioServiceSID = project.TwilioServiceSID,
                    TwilioAccountSID = project.TwilioAccountSID,
                    TwilioAuthToken = project.TwilioAuthToken,
                    XML = oldProject.XML,
                    OwnerId = user.Id
                };

                // TODO create IAM user for project?

                // create API Gateway
                // TODO convert this to use prod url after sufficient debugging
                ApiGatewayRestApi result = await _apiGatewayService.CreateApiGateway(
                    newProject.Id.ToString(),
                    "https://localhost:44305/Projects/Details/" + newProject.Id);
                newProject.RestApiId = result.RestApiId;
                _context.Add(newProject);

                // create s3 bucket
                bool bucketCreateSucceeded = await _s3Service.CreateOrUpdateBucket(newProject.S3BucketName(), S3CannedACL.Private);
                if (!bucketCreateSucceeded)
                {
                    string message = "Unable to create an S3 bucket.";
                    // TODO log message to ILogger
                    return RedirectToAction("Error", "Home", new { message });
                }

                // TODO create DynamoDB table
                await _dynamoDbService.Create(newProject.Id);

                // add Twilio integration
                Service twilioService = await _context.Services.FirstOrDefaultAsync(x => x.Name == "Twilio");
                if (twilioService == null)
                {
                    string message = "Unable to locate the Twilio service.";
                    // TODO log message to ILogger
                    return RedirectToAction("Error", "Home", new { message });
                }
                _context.Integrations.Add(new Integration
                {
                    Id = Guid.NewGuid(),
                    ProjectId = newProject.Id,
                    ServiceId = twilioService.Id
                });

                // save changes
                await _context.SaveChangesAsync();
                
                return RedirectToAction("Dashboard", "Dashboard");
            }

            return View(project);
        }
    }
}
