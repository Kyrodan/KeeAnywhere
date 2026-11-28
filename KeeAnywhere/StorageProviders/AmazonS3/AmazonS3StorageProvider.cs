using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.AmazonS3
{
    public class AmazonS3StorageProvider : IStorageProvider
    {
        private readonly AccountConfiguration _account;
        private readonly AmazonS3Client _api;

        public AmazonS3StorageProvider(AccountConfiguration account)
        {
            _account = account;
        }

        public async Task<Stream> Load(string path)
        {
            using (var api = AmazonS3Helper.GetApi(_account))
            {
                string bucket;
                string filename;

                GetBucketAndKey(path, out bucket, out filename);

                var request = new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = filename
                };

                var response = await api.GetObjectAsync(request);
                return response.ResponseStream;
            }
        }

        public async Task Save(Stream stream, string path)
        {
            using (var api = AmazonS3Helper.GetApi(_account))
            {
                string bucket;
                string filename;

                GetBucketAndKey(path, out bucket, out filename);

                // No check for a parent "folder": S3 has no real folders - folders listed from key prefixes
                // often have no "folder/" object, and PutObject creates the key anyway
                var request = new PutObjectRequest
                {
                    BucketName = bucket,
                    Key = filename,
                    InputStream = stream,
                    // Many S3 compatible services (e.g. Cloudflare R2, Synology C2) don't support streaming SigV4 (STREAMING-AWS4-HMAC-SHA256-PAYLOAD)
                    DisablePayloadSigning = IsCustomHttpsEndpoint()
                };

                var response = await api.PutObjectAsync(request);

                if (response == null)
                    throw new InvalidOperationException("Save to Amazon S3 failed.");

            }
        }

        public async Task Copy(string sourcePath, string destPath)
        {
            using (var api = AmazonS3Helper.GetApi(_account))
            {
                string sourceBucket;
                string sourceFilename;
                string destBucket;
                string destFilename;

                GetBucketAndKey(sourcePath, out sourceBucket, out sourceFilename);
                GetBucketAndKey(destPath, out destBucket, out destFilename);

                var response = await api.CopyObjectAsync(sourceBucket, sourceFilename, destBucket, destFilename);

                if (response == null)
                    throw new InvalidOperationException("Copy for Amazon S3 failed.");
            }
        }

        public async Task Delete(string path)
        {
            using (var api = AmazonS3Helper.GetApi(_account))
            {
                string bucket;
                string filename;

                GetBucketAndKey(path, out bucket, out filename);

                var response = await api.DeleteObjectAsync(bucket, filename);

                if (response == null)
                    throw new InvalidOperationException("Delete for Amazon S3 failed.");
            }
        }

        public async Task<StorageProviderItem> GetRootItem()
        {
            return new StorageProviderItem
            {
                Id = "/",
                Type = StorageProviderItemType.Folder
            };
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent)
        {
            using (var api = AmazonS3Helper.GetApi(_account))
            {
                string fixedBucket;
                if (parent.Id == "/" && _account.AdditionalSettings.TryGetValue("Bucket", out fixedBucket) && !string.IsNullOrEmpty(fixedBucket))
                {
                    // #248: only the configured bucket (or folder), so s3:ListAllMyBuckets isn't needed
                    return new[]
                    {
                        new StorageProviderItem
                        {
                            Id = fixedBucket,
                            Name = fixedBucket,
                            Type = StorageProviderItemType.Folder,
                            ParentReferenceId = parent.Id
                        }
                    };
                }

                if (parent.Id == "/")
                {
                    var response = await api.ListBucketsAsync();
                    
                    var items = response.Buckets.Select(_ => new StorageProviderItem
                    {
                        Id = _.BucketName,
                        Name = _.BucketName,
                        Type = StorageProviderItemType.Folder,
                        ParentReferenceId = parent.Id
                    });

                    return items.ToArray();
                }
                else
                {
                    string bucket;
                    string prefix;
                    GetBucketAndKey(parent.Id, out bucket, out prefix);

                    // A folder path without trailing slash (e.g. "bucket/folder" from the backup rotation) would only
                    // list the folder itself as common prefix "folder/", but not its content (#469)
                    if (!string.IsNullOrEmpty(prefix) && !prefix.EndsWith("/"))
                        prefix += "/";

                    var request = new ListObjectsV2Request
                    {
                        BucketName = bucket,
                        Prefix = prefix,
                        Delimiter = "/",
                    };

                    var items = new List<StorageProviderItem>();
                    ListObjectsV2Response response;

                    do
                    {
                        response = await api.ListObjectsV2Async(request);

                        items.AddRange(response.CommonPrefixes.Select(folderName => new StorageProviderItem
                        {
                            Id = bucket + "/" + folderName,
                            Name = folderName.RemovePrefix(prefix).RemoveTrailingSlash(),
                            Type = StorageProviderItemType.Folder,
                            ParentReferenceId = parent.Id,
                        }));

                        foreach (var o in response.S3Objects)
                        {
                            var normalized = o.Key.RemovePrefix(prefix);

                            if (string.IsNullOrEmpty(normalized) || normalized.EndsWith("/")) // Is Parent Folder (Dummy Item)? => ignore
                            {
                                continue;
                            }

                            items.Add(new StorageProviderItem
                            {
                                Id = bucket + "/" + o.Key,
                                Name = normalized,
                                Type = StorageProviderItemType.File,
                                ParentReferenceId = parent.Id,
                                LastModifiedDateTime = o.LastModified
                            });
                        }

                        request.ContinuationToken = response.NextContinuationToken;

                    } while (response.IsTruncated);

                    return items.ToArray();
                }
            }
        }

        public Task<IEnumerable<StorageProviderItem>> GetChildrenByParentPath(string path)
        {
            return this.GetChildrenByParentItem(new StorageProviderItem { Id = path });
        }


        private bool IsCustomHttpsEndpoint()
        {
            string endpointUrl;
            if (!_account.AdditionalSettings.TryGetValue("EndpointURL", out endpointUrl) || string.IsNullOrWhiteSpace(endpointUrl))
                return false;

            // Unsigned payloads are only safe with HTTPS (integrity is protected by TLS)
            return endpointUrl.Trim().StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }

        private static void GetBucketAndKey(string path, out string bucket, out string filename)
        {
            bucket = path;
            filename = null;

            if (!path.Contains('/')) return;

            bucket = path.Substring(0, path.IndexOf('/'));
            filename = path.Substring(path.IndexOf('/') + 1);
        }

        public bool IsFilenameValid(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;

            char[] invalidChars = { '/', '\\'};
            return filename.All(c => c >= 32 && !invalidChars.Contains(c));
        }

    }
}
