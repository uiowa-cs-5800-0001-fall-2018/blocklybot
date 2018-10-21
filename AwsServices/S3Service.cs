using System;
using System.Net;
using System.Net.Http;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using NLog;

namespace AwsServices
{
    public class S3Service
    {
        private readonly AmazonS3Client _client;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public S3Service(string awsAccessKey, string awsSecretKey, RegionEndpoint awsRegion)
        {
            _client = new AmazonS3Client(new BasicAWSCredentials(awsAccessKey, awsSecretKey), awsRegion);
        }

        /// <summary>
        ///     Create a bucket with the given name
        /// </summary>
        /// <remarks>Will return true for buckets that already exist</remarks>
        /// <param name="bucketName">Name of the bucket to create</param>
        /// <returns><code>true</code> if creating the bucket succeeded, otherwise <code>false</code></returns>
        public bool CreateBucket(string bucketName)
        {
            try
            {
                PutBucketResponse bucket = _client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true,
                    CannedACL = S3CannedACL.PublicRead
                }).Result;

                return bucket.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }
        }

        /// <summary>
        ///     Delete the bucket with the given name
        /// </summary>
        /// <remarks>
        ///     All objects (including all object versions and delete markers) in the bucket must be deleted before the bucket
        ///     itself can be deleted.
        /// </remarks>
        /// <param name="bucketName">Name of the bucket to delete</param>
        /// <returns><code>true</code> if bucket was successfully deleted, otherwise <code>false</code></returns>
        public bool DeleteBucket(string bucketName)
        {
            try
            {
                DeleteBucketResponse response = _client.DeleteBucketAsync(new DeleteBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                }).Result;

                return response.HttpStatusCode == HttpStatusCode.NoContent;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }
        }

        /// <summary>
        ///     Check if a bucket with the given name exists
        /// </summary>
        /// <param name="bucketName">Name of the bucket to be created</param>
        /// <returns><code>true</code> if the bucket exists, otherwise <code>false</code></returns>
        public bool CheckIfBucketExists(string bucketName)
        {
            try
            {
                string url = _client.GetPreSignedURL(new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Expires = DateTime.UtcNow.AddMinutes(3),
                    Verb = HttpVerb.HEAD
                });

                HttpClient httpClient = new HttpClient();

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, new Uri(url));

                HttpResponseMessage response = httpClient.SendAsync(request).Result;

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }
        }
    }
}