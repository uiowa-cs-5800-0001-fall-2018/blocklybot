using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using Amazon;
using Amazon.APIGateway.Model;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using BlockBot.AwsServices.Models;
using BlockBot.AwsServices.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlockBot.Tests.AwsServicesTests
{
    [TestClass]
    public class ApiGatewayServiceTests
    {
        private static LambdaService _lambdaService;
        private static S3Service _s3Service;
        private static ApiGatewayService _apiGatewayService;
        private const string Bucket = "harley-waldstein-test-lambda-bucket";
        private const string ZipKey = "test-deploy-1.zip";
        private const string FunctionName = "unit-test-function";
        private const string FunctionDescription = "TODO";
        private const string Role = "arn:aws:iam::687311858360:role/service-role/fundamentals_chatbot";
        private static string _functionArn;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            SharedCredentialsFile credentialsFile = new SharedCredentialsFile();
            credentialsFile.TryGetProfile("fundamentals-chatbot", out CredentialProfile profile);

            _s3Service = new S3Service(profile.GetAWSCredentials(credentialsFile), RegionEndpoint.USEast1);

            _lambdaService = new LambdaService(profile.GetAWSCredentials(credentialsFile), RegionEndpoint.USEast1);

            _apiGatewayService = new ApiGatewayService(profile.GetAWSCredentials(credentialsFile), RegionEndpoint.USEast1);

            // delete bucket if exists
            if (_s3Service.CheckIfBucketOrObjectExists(Bucket).Result)
            {
                foreach (S3Object s3Object in _s3Service.ReadBucket(Bucket).Result)
                {
                    bool objectDeleteSucceeded = _s3Service.DeleteObject(Bucket, s3Object.Key).Result;
                    // TODO error handling?
                }

                bool bucketDeleteSucceeded = _s3Service.DeleteBucket(Bucket).Result;
                // TODO error handling?
            }

            // create bucket
            bool bucketCreateSucceeded = _s3Service.CreateOrUpdateBucket(Bucket, S3CannedACL.PublicRead).Result;

            // lambda file contents
            string fileName = "index.js";
            string fileContents =
                "exports.handler = async (event) => {\n" +
                "    const response = {\n" +
                "        statusCode: 200,\n" +
                "        body: JSON.stringify('Hello from Lambda!')\n" +
                "    };\n" +
                "    return response;\n" +
                "};";

            // create lambda zip
            using (MemoryStream outStream = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    ZipArchiveEntry fileInArchive = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                    using (Stream entryStream = fileInArchive.Open())
                    using (MemoryStream fileToCompressStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContents)))
                    {
                        fileToCompressStream.CopyTo(entryStream);
                    }
                }

                bool objectCreateSucceeded =
                    _s3Service.CreateOrUpdateObject(Bucket, ZipKey, outStream, S3CannedACL.PublicRead).Result;
            }

            // delete lambda if exists
            bool lambdaExists1 = _lambdaService.CheckIfLambdaExists(FunctionName).Result;
            if (lambdaExists1)
            {
                bool lambdaDeleteSucceeded = _lambdaService.DeleteLambda(FunctionName).Result;
            }
            
            // create lambda
            _functionArn = _lambdaService.CreateLambda(FunctionName, FunctionDescription, Role, Bucket, ZipKey).Result;
            Assert.IsTrue(_functionArn.Length > 0);
            // exception will be thrown if create fails
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            bool lambdaDeleteSucceeded = _lambdaService.DeleteLambda(FunctionName).Result;
            Assert.IsTrue(lambdaDeleteSucceeded);

            foreach (S3Object s3Object in _s3Service.ReadBucket(Bucket).Result)
            {
                bool objectDeleteSucceeded = _s3Service.DeleteObject(Bucket, s3Object.Key).Result;
                // TODO error handling?
            }

            bool bucketDeleteSucceeded = _s3Service.DeleteBucket(Bucket).Result;
        }

        [TestMethod]
        public void CreateAndDeleteApi()
        {
            ApiGatewayRestApi restApi = _apiGatewayService.CreateApiGateway("unit-test", "API for unit tests").Result;

            ApiGatewayResource resource1 = _apiGatewayService.CreateResourceMappedToLambda(restApi.RestApiId, "twilio", _functionArn, Role).Result;

            ApiGatewayResource resource2 = _apiGatewayService.CreateResourceMappedToLambda(restApi.RestApiId, "twilio-v2", _functionArn, Role).Result;

            CreateDeploymentResponse deploy1 = _apiGatewayService.DeployRestApi(restApi.RestApiId).Result;

            ApiGatewayResourceDelete deleteResourceResult = _apiGatewayService.DeleteResourceMappedToLambda(resource1.RestApiId,
                resource1.ResourceId).Result;

            CreateDeploymentResponse deploy2 = _apiGatewayService.DeployRestApi(restApi.RestApiId).Result;

            ApiGatewayRestApiDelete deleteApiGatewaySucceeded = _apiGatewayService.DeleteApiGateway(restApi.RestApiId).Result;
        }
    }
}