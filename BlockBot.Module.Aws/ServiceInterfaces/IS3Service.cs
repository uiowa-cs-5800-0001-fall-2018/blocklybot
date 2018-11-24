using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace BlockBot.Module.Aws.ServiceInterfaces
{
    public interface IS3Service
    {
        /// <summary>
        ///     Create a bucket with the given name
        /// </summary>
        /// <remarks>Will return true for buckets that already exist</remarks>
        /// <param name="bucketName">Name of the bucket to create</param>
        /// <param name="s3CannedAcl">The ACL for the bucket</param>
        /// <returns><code>true</code> if creating the bucket succeeded, otherwise <code>false</code></returns>
        Task<bool> CreateOrUpdateBucket(string bucketName, S3CannedACL s3CannedAcl);

        /// <summary>
        ///     Delete the bucket with the given name
        /// </summary>
        /// <remarks>
        ///     All objects (including all object versions and delete markers) in the bucket must be deleted before the bucket
        ///     itself can be deleted.
        /// </remarks>
        /// <param name="bucketName">Name of the bucket to delete</param>
        /// <returns><code>true</code> if bucket was successfully deleted, otherwise <code>false</code></returns>
        Task<bool> DeleteBucket(string bucketName);

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
        Task<bool> CheckIfBucketOrObjectExists(string bucketName, string keyName = null);

        Task<IEnumerable<S3Object>> ReadBucket(string bucketName, int maxResults = 1000,
            string startAfterKey = null, string keyPrefix = null);

        Task<bool> CreateOrUpdateObject(string bucketName, string keyName, Stream inputStream, S3CannedACL cannedAcl);

        Task<Stream> ReadObject(string bucketName, string keyName);

        Task<bool> DeleteObject(string bucketName, string keyName);
    }
}