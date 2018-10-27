using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using BlockBot.AwsServices.ServiceInterfaces;
using NLog;

namespace BlockBot.AwsServices.Services
{
    public class S3Service : IS3Service
    {
        private readonly AmazonS3Client _client;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public S3Service(AWSCredentials credentials, RegionEndpoint awsRegion)
        {
            _client = new AmazonS3Client(credentials, awsRegion);
        }

        /// <summary>
        ///     Create a bucket with the given name
        /// </summary>
        /// <remarks>Will return true for buckets that already exist</remarks>
        /// <param name="bucketName">Name of the bucket to create</param>
        /// <param name="s3CannedAcl">The ACL for the bucket</param>
        /// <returns><code>true</code> if creating the bucket succeeded, otherwise <code>false</code></returns>
        public async Task<bool> CreateOrUpdateBucket(string bucketName, S3CannedACL s3CannedAcl)
        {
            try
            {
                PutBucketResponse bucket = await _client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true,
                    CannedACL = s3CannedAcl
                });

                return bucket.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
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
        public async Task<bool> DeleteBucket(string bucketName)
        {
            try
            {
                DeleteBucketResponse response = await _client.DeleteBucketAsync(new DeleteBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                });

                return response.HttpStatusCode == HttpStatusCode.NoContent;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        /// <summary>
        ///     Check if a bucket with the given name exists. If a key is passed, this method will
        ///     instead check if the specified key exists in the specified bucket.
        /// </summary>
        /// <param name="bucketName">Name of the bucket to search for</param>
        /// <param name="keyName">Key of the object in the bucket to search for</param>
        /// <returns>
        ///     <code>true</code> if the bucket exists, otherwise <code>false</code>. If
        ///     a keyName is passed, <code>true</code> if the key and bucket exist, otherwise
        ///     <code>false</code>.
        /// </returns>
        public async Task<bool> CheckIfBucketOrObjectExists(string bucketName, string keyName = null)
        {
            try
            {
                string url = _client.GetPreSignedURL(new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    Expires = DateTime.UtcNow.AddMinutes(3),
                    Verb = HttpVerb.HEAD
                });

                HttpClient httpClient = new HttpClient();

                HttpResponseMessage response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public async Task<IEnumerable<S3Object>> ReadBucket(string bucketName, int maxResults = 1000,
            string startAfterKey = null, string keyPrefix = null)
        {
            try
            {
                ListObjectsV2Response result = await _client.ListObjectsV2Async(new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    StartAfter = startAfterKey,
                    MaxKeys = maxResults,
                    Prefix = keyPrefix
                    // TODO consider populating FetchOwner attribute
                });

                return result.S3Objects;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public async Task<bool> CreateOrUpdateObject(string bucketName, string keyName, Stream inputStream, S3CannedACL cannedAcl)
        {
            try
            {
                PutObjectResponse result = await _client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    CannedACL = cannedAcl,
                    InputStream = inputStream
                });

                return result.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public async Task<Stream> ReadObject(string bucketName, string keyName)
        {
            try
            {
                GetObjectResponse result = await _client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                });

                return result.ResponseStream;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        public async Task<bool> DeleteObject(string bucketName, string keyName)
        {
            // TODO look into bucket lifecycles and delete markers
            try
            {
                DeleteObjectResponse result = await _client.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                });

                return result.HttpStatusCode == HttpStatusCode.NoContent;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }
    }
}