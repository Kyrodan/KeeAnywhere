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

        public async Task<bool> Save(Stream stream, string path)
        {
            using (var api = AmazonS3Helper.GetApi(_account))
            {
                string bucket;
                string filename;

                GetBucketAndKey(path, out bucket, out filename);

                try // Does parent folder exists?
                {
                    var folderName = CloudPath.GetDirectoryName(filename);
                    var getResponse = await api.GetObjectMetadataAsync(bucket, folderName + "/");
                }
                catch (AmazonS3Exception ex)
                {
                    if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return false;

                    //status wasn't not found, so throw the exception
                    throw;
                }

                var request = new PutObjectRequest
                {
                    BucketName = bucket,
                    Key = filename,
                    InputStream = stream
                };

                var response = await api.PutObjectAsync(request);

                return response != null;
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
                    string filename;
                    GetBucketAndKey(parent.Id, out bucket, out filename);
                    var filenameLength = string.IsNullOrEmpty(filename) ? 0 : filename.Length;

                    var request = new ListObjectsV2Request
                    {
                        BucketName = bucket,
                        Prefix = filename,
                    };

                    var items = new List<StorageProviderItem>();
                    ListObjectsV2Response response;
                    var finished = false;

                    do
                    {
                        response = await api.ListObjectsV2Async(request);
                        foreach (var o in response.S3Objects)
                        {
                            var normalized = o.Key.Substring(filenameLength);
                            if (string.IsNullOrEmpty(normalized))
                                continue;

                            if (normalized.EndsWith("/"))
                            {
                                normalized = normalized.Remove(normalized.Length - 1);
                            }


                            if (normalized.Contains('/'))
                            {
                                finished = true;
                                break;
                            }

                            items.Add(new StorageProviderItem
                            {
                                Id = bucket + "/" + o.Key,
                                Name = normalized,
                                Type = o.Size > 0 ? StorageProviderItemType.File : StorageProviderItemType.Folder,
                                ParentReferenceId = parent.Id,
                            });
                        }

                        if (finished)
                            break;

                        request.ContinuationToken = response.NextContinuationToken;

                    } while (response.IsTruncated);

                    return items.ToArray();
                }
            }
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
