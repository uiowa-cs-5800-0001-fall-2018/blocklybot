using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using BlockBot.Common.Data;
using BlockBot.module.Integrations.Services;
using BlockBot.Module.Aws.ServiceInterfaces;
using BlockBot.Module.Google.Services;
using BlockBot.Module.Twilio.ServiceInterfaces;
using BlockBot.Web.Models;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlockBot.Web.Controllers
{
    public class WorkspaceController : Controller
    {
        private readonly GoogleCalendarService _googleCalendarService;
        private readonly IntegrationCreationService _integrationCreationService;
        private readonly ILogger<WorkspaceController> _logger;
        private readonly ApplicationUserManager _userManager;
        private ApplicationDbContext _context;

        public WorkspaceController(ApplicationDbContext context,
            ApplicationUserManager userManager,
            IApiGatewayService apiGatewayService,
            ILambdaService lambdaService,
            IS3Service s3Service,
            ITwilioService twilioService,
            IntegrationCreationService integrationCreationService,
            ILogger<WorkspaceController> logger,
            GoogleCalendarService googleCalendarService)
        {
            _context = context;
            _userManager = userManager;
            _integrationCreationService = integrationCreationService;
            _logger = logger;
            _googleCalendarService = googleCalendarService;
        }

        /// <summary>
        /// </summary>
        /// <param name="id">The project id</param>
        /// <returns></returns>
        public async Task<IActionResult> Workspace(Guid id)
        {
            ViewData["Message"] = "A workspace for editing programs";

            Project project = await _context.Projects
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

            var model = new WorkspaceModel
            {
                Project = project
            };
            ApplicationUserClaim claim = user.Claims.FirstOrDefault(x => x.ClaimType == "GoogleRefreshToken");
            if (claim != null)
            {
                // TODO change this to load db context some other way eventually
                model.CalendarList = _googleCalendarService
                    .ListCalendars(ref _context, user.NormalizedUserName)
                    .Items
                    .Where(x => x.AccessRole == "writer" || x.AccessRole == "owner")
                    .ToList();
            }


            return View(model);
        }

        [HttpPut]
        public async Task<IActionResult> PutProjectXml(Guid id, string xml)
        {
            Project project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound($"Unable to load project with ID '{id}'.");
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

            project.XML = xml;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> PutProjectCode(Guid id, string code)
        {
            Project project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound($"Unable to load project with ID '{id}'.");
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
                // TODO use project-specific role? use user-specific role?
                string role = "arn:aws:iam::687311858360:role/service-role/fundamentals_chatbot";
                await Task.WhenAll(
                    _context
                        .Integrations
                        .Where(x => x.ProjectId == id)
                        .Select(integration =>
                            _integrationCreationService
                                .Integrate(
                                    integration.Service.Name,
                                    id,
                                    RegionEndpoint.USEast1,
                                    role,
                                    project.RestApiId,
                                    project.S3BucketName(),
                                    code)));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"There was an error creating/updating integrations for project id '{id}'.");
                return NoContent();
            }

            return NoContent();
        }
    }
}