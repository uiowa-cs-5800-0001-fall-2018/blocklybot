using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.APIGateway.Model;
using Amazon.S3;
using BlockBot.Module.Aws.Models;
using BlockBot.Module.Aws.ServiceInterfaces;
using BlockBot.Module.Twilio.ServiceInterfaces;
using BlockBot.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlockBot.Web.Controllers
{
    public class WorkspaceController : Controller
    {
        private readonly IApiGatewayService _apiGatewayService;
        private readonly ApplicationDbContext _context;
        private readonly ILambdaService _lambdaService;
        private readonly ILogger<WorkspaceController> _logger;
        private readonly IS3Service _s3Service;
        private readonly ITwilioService _twilioService;
        private readonly ApplicationUserManager _userManager;

        public WorkspaceController(ApplicationDbContext context,
            ApplicationUserManager userManager,
            IApiGatewayService apiGatewayService,
            ILambdaService lambdaService,
            IS3Service s3Service,
            ITwilioService twilioService,
            ILogger<WorkspaceController> logger)
        {
            _context = context;
            _userManager = userManager;
            _apiGatewayService = apiGatewayService;
            _lambdaService = lambdaService;
            _s3Service = s3Service;
            _twilioService = twilioService;
            _logger = logger;
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

            return View(project);
        }

        [HttpPut]
        public async Task<IActionResult> PutProjectXml(int id, string xml)
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
        public async Task<IActionResult> PutProjectCode(int id, string code)
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

            // calculate magic strings
            string serviceName = "twilio";
            string integrationName = user.Id + "-" + project.Id + "-" + serviceName;
            string outKey = integrationName + ".zip";

            string templateKey = serviceName + ".zip";
            string bucket = "fundamentals-chatbot";
            string fileName = "index.js";
            string role = "arn:aws:iam::687311858360:role/service-role/fundamentals_chatbot";

            try
            {
                // create deployment zip
                using (MemoryStream memoryStream = new MemoryStream())
                using (Stream outStream = await _s3Service.ReadObject(bucket, templateKey))
                {
                    outStream.CopyTo(memoryStream);
                    using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Update, true))
                    {
                        string fileContents;
                        ZipArchiveEntry oldEntry = archive.GetEntry(fileName);
                        using (StreamReader entryStream = new StreamReader(oldEntry.Open()))
                        {
                            fileContents = entryStream.ReadToEnd();
                        }

                        oldEntry.Delete();

                        // TODO perform replacement

                        int startIndex = fileContents.IndexOf("START_CODE_PLACEHOLDER", StringComparison.Ordinal);
                        string startContent = fileContents.Substring(0, startIndex + 22);

                        int endIndex = fileContents.IndexOf("END_CODE_PLACEHOLDER", StringComparison.Ordinal);
                        string endContent = fileContents.Substring(endIndex - 3);

                        fileContents = startContent + "\n" + code + "\n" + endContent;

                        ZipArchiveEntry newEntry = archive.CreateEntry(fileName);
                        using (Stream entryStream = newEntry.Open())
                        {
                            using (MemoryStream newFileContentsStream =
                                new MemoryStream(Encoding.UTF8.GetBytes(fileContents)))
                            {
                                newFileContentsStream.CopyTo(entryStream);
                            }
                        }
                    }

                    bool objectCreateSucceeded =
                        await _s3Service.CreateOrUpdateObject(bucket, outKey, memoryStream, S3CannedACL.PublicRead);
                }

                // create/update function with deployment zip
                string functionArn;
                bool lambdaExists = await _lambdaService.CheckIfLambdaExists(integrationName);
                if (lambdaExists)
                {
                    functionArn = await _lambdaService.UpdateLambda(integrationName, bucket, outKey);
                }
                else
                {
                    functionArn = await _lambdaService.CreateLambda(integrationName, "TODO add permalink to project",
                        role, bucket, outKey);
                }

                // TODO look into creating an update resource method
                // delete old API Gateway resource
                if (lambdaExists)
                {
                    await _apiGatewayService.DeleteResourceMappedToLambda(project.RestApiId, serviceName);
                }

                // create new API Gateway resource
                ApiGatewayResource x =
                    await _apiGatewayService.CreateResourceMappedToLambda(project.RestApiId, serviceName, functionArn,
                        role);

                // deploy changes
                CreateDeploymentResponse y = await _apiGatewayService.DeployRestApi(project.RestApiId);

                // TODO replace with region from IConfiguration
                string newApi =
                    $"https://{project.RestApiId}.execute-api.{RegionEndpoint.USEast1.SystemName}.amazonaws.com/default/{serviceName}";

                _twilioService.UpdateServiceProcessingUrl(newApi, "MG2ae64c23b7ba10eb5aecff49998e5ec9");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception updating integration infrastructure");
                throw;
            }

            // TODO record the creation of resources
            return NoContent();
        }
    }
}