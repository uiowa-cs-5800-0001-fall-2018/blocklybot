using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.APIGateway.Model;
using Amazon.S3;
using BlockBot.Module.Aws.Models;
using BlockBot.Module.Aws.Services;
using BlockBot.Module.BlockBot.Services;
using BlockBot.Module.Twilio.Services;

namespace BlockBot.Module.Integrations.Services
{
    public class IntegrationCreationService
    {
        private readonly ApiGatewayService _apiGatewayService;
        private readonly LambdaService _lambdaService;
        private readonly S3Service _s3Service;
        private readonly BlockBotIntegrationCreationService _blockBotIntegrationCreationService;
        private readonly TwilioIntegrationCreationService _twilioIntegrationCreationService;

        public IntegrationCreationService(
            ApiGatewayService apiGatewayService,
            LambdaService lambdaService,
            S3Service s3Service,
            BlockBotIntegrationCreationService blockBotIntegrationCreationService,
            TwilioIntegrationCreationService twilioIntegrationCreationService)
        {
            _apiGatewayService = apiGatewayService;
            _lambdaService = lambdaService;
            _s3Service = s3Service;
            _blockBotIntegrationCreationService = blockBotIntegrationCreationService;
            _twilioIntegrationCreationService = twilioIntegrationCreationService;
        }

        public async Task Integrate(string serviceName, Guid projectId, string iamRole, string restApiId,
            string targetBucket, string code)
        {
            // magic strings
            string sourceBucket = "blockbot-integration-templates";
            string fileName = "index.js";

            serviceName = serviceName.ToLowerInvariant();
            string integrationName = projectId + "-" + serviceName;
            string templateKey = serviceName + ".zip";
            string outKey = integrationName + ".zip";


            // create deployment zip
            using (MemoryStream memoryStream = new MemoryStream())
            using (Stream outStream = await _s3Service.ReadObject(sourceBucket, templateKey))
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
                        await _s3Service.CreateOrUpdateObject(targetBucket, outKey, memoryStream,
                            S3CannedACL.Private);
                }

                // create/update function with deployment zip
                string functionArn;
                bool lambdaExists = await _lambdaService.CheckIfLambdaExists(integrationName);
                if (lambdaExists)
                {
                    functionArn = await _lambdaService.UpdateLambda(integrationName, targetBucket, outKey);
                }
                else
                {
                    functionArn = await _lambdaService.CreateLambda(integrationName, "https://blockbot.io/Projects/Details/" + projectId,
                        iamRole, targetBucket, outKey);
                }

                // TODO look into creating an update resource method
                // delete old API Gateway resource
                if (lambdaExists)
                {
                    await _apiGatewayService.DeleteResourceMappedToLambda(restApiId, serviceName);
                }

                // create new API Gateway resource
                ApiGatewayResource x =
                    await _apiGatewayService.CreateResourceMappedToLambda(restApiId, serviceName, functionArn,
                        iamRole);

                // deploy changes
                CreateDeploymentResponse y = await _apiGatewayService.DeployRestApi(restApiId);

                // TODO replace with region from IConfiguration
                string newApi =
                    $"https://{restApiId}.execute-api.{RegionEndpoint.USEast1.SystemName}.amazonaws.com/default/{serviceName}";

                // TODO log info on integrations in database

                // perform specific integration actions
                if (serviceName == BlockBotIntegrationCreationService.ServiceName())
                {
                    // no special integrations -- should "just work"
                    // TODO consider deleting the BlockBot integration creation service
                    // TODO consider moving specific integrations into this service as functions
                }
                else if (serviceName == TwilioIntegrationCreationService.ServiceName())
                {
                    // TODO check/perform initialization if necessary
                    await _twilioIntegrationCreationService.Integrate(newApi);
                }
                // TODO there is probably a better way to do this / is this extensible to more integrations?
        }
    }
}