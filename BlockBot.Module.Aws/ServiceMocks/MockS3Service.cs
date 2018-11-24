using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using BlockBot.Module.Aws.ServiceInterfaces;

namespace BlockBot.Module.Aws.ServiceMocks
{
    internal class MockS3Service : IS3Service
    {
        public Task<bool> CreateOrUpdateBucket(string bucketName, S3CannedACL s3CannedAcl)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteBucket(string bucketName)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> CheckIfBucketOrObjectExists(string bucketName, string keyName = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<S3Object>> ReadBucket(string bucketName, int maxResults = 1000, string startAfterKey = null, string keyPrefix = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> CreateOrUpdateObject(string bucketName, string keyName, Stream inputStream, S3CannedACL cannedAcl)
        {
            throw new System.NotImplementedException();
        }

        public Task<Stream> ReadObject(string bucketName, string keyName)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteObject(string bucketName, string keyName)
        {
            throw new System.NotImplementedException();
        }
    }
}