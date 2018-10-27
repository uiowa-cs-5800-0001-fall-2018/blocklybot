using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class S3ServiceTests
    {
        private static S3Service _s3Service;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            SharedCredentialsFile credentialsFile = new SharedCredentialsFile();
            credentialsFile.TryGetProfile("fundamentals-chatbot", out CredentialProfile profile);

            _s3Service = new S3Service(profile.GetAWSCredentials(credentialsFile), RegionEndpoint.USEast1);
        }

        [TestMethod]
        public void CrudBucketAndObject()
        {
            string bucket = "harley-waldstein-test-1000";
            string file = "test.txt";
            string fileContents = "test file contents";

            // TODO test that functionArns can be used in place of functionNames

            // Exist bucket false
            bool bucketExists1 = _s3Service.CheckIfBucketOrObjectExists(bucket).Result;
            Assert.IsFalse(bucketExists1);

            // List bucket error
            try
            {
                IEnumerable<S3Object> bucketContents1 = _s3Service.ReadBucket(bucket).Result;
            }
            catch (AggregateException)
            {
                // do nothing, this exception was expected
            }
            catch (Exception)
            {
                Assert.Fail("Wrong exception received.");
            }

            // Create bucket
            bool bucketCreateSucceeded = _s3Service.CreateOrUpdateBucket(bucket, S3CannedACL.PublicRead).Result;
            Assert.IsTrue(bucketCreateSucceeded);

            // Exist bucket true
            bool bucketExists2 = _s3Service.CheckIfBucketOrObjectExists(bucket).Result;
            Assert.IsTrue(bucketExists2);

            // List bucket empty
            IEnumerable<S3Object> bucketContents2 = _s3Service.ReadBucket(bucket).Result;
            Assert.IsFalse(bucketContents2.Any());

            // Exist file false
            bool fileExists1 = _s3Service.CheckIfBucketOrObjectExists(bucket, file).Result;
            Assert.IsFalse(fileExists1);

            // Create file
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContents)))
            {
                bool fileWriteSucceeded =
                    _s3Service.CreateOrUpdateObject(bucket, file, stream, S3CannedACL.PublicRead).Result;
                Assert.IsTrue(fileWriteSucceeded);
            }

            // Exist file true
            bool fileExists2 = _s3Service.CheckIfBucketOrObjectExists(bucket, file).Result;
            Assert.IsTrue(fileExists2);

            // List bucket contains file
            IEnumerable<S3Object> bucketContents3 = _s3Service.ReadBucket(bucket).Result;
            Assert.IsTrue(bucketContents3.Count() == 1);

            // Validate file contents
            using (Stream stream = _s3Service.ReadObject(bucket, file).Result)
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int) stream.Length);
                string actualFileContents = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                Assert.AreEqual(fileContents, actualFileContents);
            }

            // Delete file
            bool fileDeleteSucceeded = _s3Service.DeleteObject(bucket, file).Result;
            Assert.IsTrue(fileDeleteSucceeded);

            // Exists file false
            bool fileExists3 = _s3Service.CheckIfBucketOrObjectExists(bucket, file).Result;
            Assert.IsFalse(fileExists3);

            // List bucket empty
            IEnumerable<S3Object> bucketContents4 = _s3Service.ReadBucket(bucket).Result;
            Assert.IsFalse(bucketContents4.Any());

            // Delete bucket
            bool bucketDeleteSucceeded = _s3Service.DeleteBucket(bucket).Result;
            Assert.IsTrue(bucketDeleteSucceeded);

            // Exist bucket false
            bool bucketExists3 = _s3Service.CheckIfBucketOrObjectExists(bucket).Result;
            Assert.IsFalse(bucketExists3);

            // List bucket error?
            try
            {
                IEnumerable<S3Object> bucketContents5 = _s3Service.ReadBucket(bucket).Result;
            }
            catch (AggregateException)
            {
                // do nothing, this exception was expected
            }
            catch (Exception)
            {
                Assert.Fail("Wrong exception received.");
            }
        }
    }
}