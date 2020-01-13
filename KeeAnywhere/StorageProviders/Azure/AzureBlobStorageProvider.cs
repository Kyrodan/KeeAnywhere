using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;

namespace KeeAnywhere.StorageProviders.Azure
{
    public class AzureBlobStorageProvider : IStorageProvider
    {
        private readonly AccountConfiguration _account;

        // Only for connection testing
        private AzureBlobStorageProvider() { }

        public AzureBlobStorageProvider(AccountConfiguration account)
        {
            _account = account;
        }

        #region IStorageProviderFileOperations implementation

        public async Task<Stream> Load(string path)
        {
            var blockBlob = await GetBlob(_account, path);

            var ms = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

        public async Task Save(Stream stream, string path)
        {
            var blockBlob = await GetBlob(_account, path);

            await blockBlob.UploadFromStreamAsync(stream);
            blockBlob.Properties.ContentType = "application/octet-stream";
            await blockBlob.SetPropertiesAsync();
        }

        public async Task Copy(string sourcePath, string destPath)
        {
            var sourceBlockBlob = await GetBlob(_account, sourcePath);
            var destBlockBlob = await GetBlob(_account, destPath);

            await destBlockBlob.StartCopyAsync(sourceBlockBlob);

            do
            {
                await destBlockBlob.FetchAttributesAsync();
                await Task.Delay(1000);
            } while (destBlockBlob.CopyState.Status == CopyStatus.Pending);

            if (destBlockBlob.CopyState.Status != CopyStatus.Success)
            {
                throw new InvalidOperationException("Copy for Azure blob storage failed.");
            }
        }

        public async Task Delete(string path)
        {
            var blockBlob = await GetBlob(_account, path);
            var deleted = await blockBlob.DeleteIfExistsAsync();

            if (!deleted)
                throw new InvalidOperationException("Delete for Azure blob storage failed.");
        }

        public bool IsFilenameValid(string filename)
        {
            if (string.IsNullOrEmpty(filename)
                || filename.Length > 1024
                || filename.Last() == '.'
                || filename.Last() == '/') return false;

            char[] invalidChars = { '/', '\\' };
            return filename.All(c => c >= 32 && !invalidChars.Contains(c));
        }

        #endregion

        #region IStorageProviderQueryOperations implementation

        [SuppressMessage("Compiler", "CS1998", Justification = "Nothing to await in this implementation.")]
        public async Task<StorageProviderItem> GetRootItem()
        {
            return GetItem("");
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent)
        {
            var blobContainer = await GetContainer(_account);
            var directory = blobContainer.GetDirectoryReference(parent.Id);
            BlobContinuationToken blobContinuationToken = null;
            var result = new List<StorageProviderItem>();

            do
            {
                var listSegment = await directory.ListBlobsSegmentedAsync(blobContinuationToken);
                result.AddRange(listSegment.Results.OfType<CloudBlobDirectory>().Select(s => new StorageProviderItem
                {
                    Id = s.Prefix,
                    Name = GetName(s.Prefix, s.Parent),
                    Type = StorageProviderItemType.Folder,
                    ParentReferenceId = parent.Id
                }));

                result.AddRange(listSegment.Results.OfType<CloudBlob>().Select(s => new StorageProviderItem
                {
                    Id = s.Uri.ToString(),
                    Name = GetName(s.Name, s.Parent),
                    Type = StorageProviderItemType.File,
                    ParentReferenceId = parent.Id
                }));

                blobContinuationToken = listSegment.ContinuationToken;
            } while (blobContinuationToken != null);

            return result;
        }

        public Task<IEnumerable<StorageProviderItem>> GetChildrenByParentPath(string path)
        {
            return this.GetChildrenByParentItem(GetItem(path));
        }

        #endregion

        // For connection testing
        public static async Task<bool> TestContainer(StorageCredentials credentials, string containerName)
        {
            var provider = new AzureBlobStorageProvider();
            var container = await provider.GetContainer(credentials, containerName);
            await container.ListBlobsSegmentedAsync(null);

            return false;
        }

        #region Private Helpers

        private StorageProviderItem GetItem(string path, StorageProviderItemType itemType = StorageProviderItemType.Folder)
        {
            return new StorageProviderItem
            {
                Id = path,
                Type = itemType
            };
        }

        private async Task<CloudBlockBlob> GetBlob(AccountConfiguration account, string path)
        {
            var blobContainer = await GetContainer(account);
            // Get specific blob reference
            var blockBlob = blobContainer.GetBlockBlobReference(path);

            return blockBlob;
        }

        private async Task<CloudBlobContainer> GetContainer(AccountConfiguration account)
        {
            var containerName = "";
            if (account.AdditionalSettings != null && account.AdditionalSettings.ContainsKey("AzureItemName"))
            {
                containerName = account.AdditionalSettings["AzureItemName"];
            }

            var blobContainer = await GetContainer(new StorageCredentials(account.Name, account.Secret), containerName);

            return blobContainer;
        }

        private async Task<CloudBlobContainer> GetContainer(StorageCredentials credentials, string containerName)
        {
            if (credentials == null) throw new ArgumentNullException(nameof(credentials));
            if (containerName == null) throw new ArgumentNullException(nameof(containerName));

            var storageAccount = new CloudStorageAccount(credentials, true);
            var cloudBlobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = cloudBlobClient.GetContainerReference(containerName);

            // Create the container and set the permission  
            if (await blobContainer.CreateIfNotExistsAsync())
            {
                await blobContainer.SetPermissionsAsync(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    }
                );
            }

            return blobContainer;
        }

        private string GetName(string id, CloudBlobDirectory parent)
        {
            return (!(String.IsNullOrEmpty(parent?.Prefix)) ? id.Replace(parent.Prefix, "") : id).TrimEnd('/');
        }

        #endregion
    }
}