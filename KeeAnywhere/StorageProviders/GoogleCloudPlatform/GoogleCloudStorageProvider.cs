using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace KeeAnywhere.StorageProviders.GoogleCloudPlatform
{
    public class GoogleCloudStorageProvider : IStorageProvider
    {
        private readonly AccountConfiguration _account;

        public GoogleCloudStorageProvider(AccountConfiguration account)
        {
            _account = account;
        }

        private StorageClient GetClient(AccountConfiguration account)
        {
            var credentials = GoogleCredential.FromJson(account.Secret);
            var client = StorageClient.Create(credentials);
            return client;
        }
        private static void GetBucketAndFilename(string path, out string bucket, out string filename)
        {
            bucket = path;
            filename = null;

            if (!path.Contains('/')) return;

            bucket = path.Substring(0, path.IndexOf('/'));
            filename = path.Substring(path.IndexOf('/') + 1);
        }

        public async Task<Stream> Load(string path)
        {
            string bucket, fileName;
            GetBucketAndFilename(path, out bucket, out fileName);

            using (var client = GetClient(_account))
            {
                var stream = new MemoryStream();
                await client.DownloadObjectAsync(bucket, fileName, stream);
                stream.Seek(0, SeekOrigin.Begin); // Rewind stream to the start so reading starts from there

                return stream;
            }
        }

        public async Task Save(Stream stream, string path)
        {
            string bucket, fileName;
            GetBucketAndFilename(path, out bucket, out fileName);

            using (var client = GetClient(_account))
            {
                await client.UploadObjectAsync(bucket, fileName, "application/octet-stream", stream);
            }
        }

        public async Task Copy(string sourcePath, string destPath)
        {
            string srcBucket, srcFileName;
            string dstBucket, dstFileName;
            GetBucketAndFilename(sourcePath, out srcBucket, out srcFileName);
            GetBucketAndFilename(destPath, out dstBucket, out dstFileName);

            using (var client = GetClient(_account))
            {
                await client.CopyObjectAsync(srcBucket, srcFileName, dstBucket, dstFileName);
            }
        }

        public async Task Delete(string path)
        {
            string bucket, fileName;
            GetBucketAndFilename(path, out bucket, out fileName);

            using (var client = GetClient(_account))
            {
                await client.DeleteObjectAsync(bucket, fileName);
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
            using (var client = GetClient(_account))
            {
                // We're at the root: list all buckets as folders.
                if (parent.Id == "/")
                {
                    var responseEnumerable = client.ListBucketsAsync(_account.AdditionalSettings["ProjectId"]);
                    var responseEnumerator = responseEnumerable.GetAsyncEnumerator();

                    var result = new List<StorageProviderItem>();

                    try
                    {
                        while (await responseEnumerator.MoveNextAsync())
                        {
                            var item = responseEnumerator.Current;
                            result.Add(new StorageProviderItem
                            {
                                Id = item.Name,
                                Name = item.Name,
                                Type = StorageProviderItemType.Folder,
                                ParentReferenceId = parent.Id
                            });
                        }
                    }
                    finally
                    {
                        responseEnumerator.DisposeAsync();
                    }

                    return result;
                }
                else
                {
                    string bucket, currentPath;
                    GetBucketAndFilename(parent.Id, out bucket, out currentPath);

                    var options = new ListObjectsOptions
                    {
                        Fields = "items(name,updated),nextPageToken", // Make this a partial response to only fetch the data we need
                        Delimiter = "/", // Limit only to current 'directory'...
                        IncludeTrailingDelimiter = true // ...but display child directories as items
                    };

                    var responseEnumerable = client.ListObjectsAsync(bucket, currentPath, options);
                    var responseEnumerator = responseEnumerable.GetAsyncEnumerator();

                    var result = new List<StorageProviderItem>();

                    try
                    {
                        while (await responseEnumerator.MoveNextAsync())
                        {
                            var item = responseEnumerator.Current;
                            var isFolder = item.Name.EndsWith("/");
                            var normalizedName = item.Name.TrimEnd(new[] { '/' }); // Remove prefix and trailing slash (if this is a folder)

                            if (currentPath != null)
                            {
                                if (item.Name == currentPath) continue; // GCS seems to also include the current 'directory' in the response, so we skip it)

                                normalizedName = normalizedName.Replace(currentPath, "");
                            }

                            result.Add(new StorageProviderItem
                            {
                                Id = bucket + "/" + item.Name, // The file name already includes the entire path
                                Name = normalizedName,
                                Type = isFolder ? StorageProviderItemType.Folder : StorageProviderItemType.File,
                                ParentReferenceId = parent.Id,
                                LastModifiedDateTime = item.Updated
                            });
                        }
                    }
                    finally
                    {
                        responseEnumerator.DisposeAsync();
                    }

                    return result;
                }
            }
        }

        public Task<IEnumerable<StorageProviderItem>> GetChildrenByParentPath(string path)
        {
            return this.GetChildrenByParentItem(new StorageProviderItem { Id = path });
        }

        public bool IsFilenameValid(string filename)
        {
            if (string.IsNullOrEmpty(filename) || filename.Length > 1024) return false;
            if (filename.Equals('.') || filename.Equals("..")) return false;
            if (filename.StartsWith(".well-known/acme-challenge/") || filename.Last() == '/') return false;

            char[] invalidChars = { '\n', '\r', '\\' };
            return filename.All(c => c >= 32 && !invalidChars.Contains(c));
        }
    }
}
