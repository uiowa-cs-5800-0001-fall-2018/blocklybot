using System.IO;
using System.IO.Compression;
using System.Text;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using BlockBot.AwsServices.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlockBot.Tests.AwsServicesTests
{
    [TestClass]
    public class LambdaServiceTests
    {
        private static LambdaService _lambdaService;
        private static S3Service _s3Service;
        private static readonly string bucket = "harley-waldstein-test-lambda-bucket";
        private static readonly string key = "text-deploy.zip";

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            SharedCredentialsFile credentialsFile = new SharedCredentialsFile();
            credentialsFile.TryGetProfile("fundamentals-chatbot", out CredentialProfile profile);

            _s3Service = new S3Service(profile.GetAWSCredentials(credentialsFile), RegionEndpoint.USEast1);

            _lambdaService = new LambdaService(profile.GetAWSCredentials(credentialsFile), RegionEndpoint.USEast1);

            // delete bucket if exists
            if (_s3Service.CheckIfBucketOrObjectExists(bucket).Result)
            {
                foreach (S3Object s3Object in _s3Service.ReadBucket(bucket).Result)
                {
                    bool objectDeleteSucceeded = _s3Service.DeleteObject(bucket, s3Object.Key).Result;
                    // TODO error handling?
                }

                bool bucketDeleteSucceeded = _s3Service.DeleteBucket(bucket).Result;
                // TODO error handling?
            }

            bool bucketCreateSucceeded = _s3Service.CreateOrUpdateBucket(bucket, S3CannedACL.PublicRead).Result;

            string fileName = "index.js";
            string fileContents =
                "exports.handler = async (event) => {\n    // TODO implement\n    const response = {\n        statusCode: 200,\n        body: JSON.stringify(\'Hello from Lambda!\')\n    };\n    return response;\n};"; //here is your file in bytes

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
                    _s3Service.CreateOrUpdateObject(bucket, key, outStream, S3CannedACL.PublicRead).Result;
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // TODO delete deploy bucket and file
        }

        [TestMethod]
        public void CreateLambda()
        {
            string functionName = "unit-test-function";
            string role = "arn:aws:iam::687311858360:role/service-role/fundamentals_chatbot";
            string description = "TODO";

            var ac = _lambdaService.CheckIfLambdaExists(functionName).Result;
            
            string a = _lambdaService.CreateLambda(functionName, description, role, bucket, key).Result;

            
            int b = 0;
        }
    }
}