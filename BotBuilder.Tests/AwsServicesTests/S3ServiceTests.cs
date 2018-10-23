using Amazon;
using Amazon.Runtime;
using BotBuilder.AwsServices.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BotBuilder.Tests.AwsServicesTests
{
    [TestClass]
    public class S3ServiceTests
    {
        private static S3Service _s3Service;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _s3Service = new S3Service(new StoredProfileAWSCredentials("fundamentals-chatbot"), RegionEndpoint.USEast1);
        }

        [TestMethod]
        public void SearchCreateAndDeleteEmptyBucket()
        {
            string bucket = "harley-waldstein-test-1000";

            bool bucketExists1 = _s3Service.CheckIfBucketExists(bucket);
            Assert.IsFalse(bucketExists1);

            bool bucketCreateSucceeded = _s3Service.CreateBucket(bucket);
            Assert.IsTrue(bucketCreateSucceeded);

            bool bucketExists2 = _s3Service.CheckIfBucketExists(bucket);
            Assert.IsTrue(bucketExists2);

            bool bucketDeleteSucceeded = _s3Service.DeleteBucket(bucket);
            Assert.IsTrue(bucketDeleteSucceeded);

            bool bucketExists3 = _s3Service.CheckIfBucketExists(bucket);
            Assert.IsFalse(bucketExists3);
        }

        [TestMethod]
        public void ListBucket()
        {
            string bucket = "fundamentals-bucket-twilio";
            _s3Service.ListBucket();
            
        }
    }
}