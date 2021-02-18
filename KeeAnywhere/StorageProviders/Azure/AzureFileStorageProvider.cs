using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.File;

namespace KeeAnywhere.StorageProviders.Azure
{
    public class AzureFileStorageProvider : IStorageProvider
    {
        private readonly AccountConfiguration _account;

        // For connection testing
        private AzureFileStorageProvider() { }

        public AzureFileStorageProvider(AccountConfiguration account)
        {
            _account = account;
        }

        #region IStorageProviderFileOperations implementation

        public async Task<Stream> Load(string path)
        {
            var file = await GetFile(_account, path);

            var ms = new MemoryStream();
            await file.DownloadToStreamAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

        public async Task Save(Stream stream, string path)
        {
            var file = await GetFile(_account, path);

            await file.UploadFromStreamAsync(stream);
            file.Properties.ContentType = "application/octet-stream";
            await file.SetPropertiesAsync();
        }

        public async Task Copy(string sourcePath, string destPath)
        {
            var sourceFile = await GetFile(_account, sourcePath);
            var destFile = await GetFile(_account, destPath);

            await destFile.StartCopyAsync(sourceFile);

            do
            {
                await destFile.FetchAttributesAsync();
                await Task.Delay(1000);
            } while (destFile.CopyState.Status == CopyStatus.Pending);

            if (destFile.CopyState.Status != CopyStatus.Success)
            {
                throw new InvalidOperationException("Copy for Azure file storage failed.");
            }
        }

        public async Task Delete(string path)
        {
            var file = await GetFile(_account, path);
            var deleted = await file.DeleteIfExistsAsync();

            if (!deleted)
                throw new InvalidOperationException("Delete for Azure file storage failed.");
        }

        public bool IsFilenameValid(string filename)
        {
            if (string.IsNullOrEmpty(filename)
                || filename.Length > 255
                || filename.Last() == '/'
                || filename == "."
                || filename == "..") return false;

            char[] invalidChars = { '/', '\\', '"', ':', '|', '<', '>', '*', '?' };
            return filename.All(c => c >= 32 && !invalidChars.Contains(c));
        }

        #endregion

        #region IStorageProviderQueryOperations implementation

        [SuppressMessage("Compiler", "CS1998", Justification = "Nothing to await in this method.")]
        public async Task<StorageProviderItem> GetRootItem()
        {
            return GetItem("");
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent)
        {
            var fileShare = await GetShare(_account);
            var rootDir = fileShare.GetRootDirectoryReference();

            var directory = rootDir;
            if (!String.IsNullOrEmpty(parent.Id))
                directory = rootDir.GetDirectoryReference(parent.Id);

            FileContinuationToken fileContinuationToken = null;
            var result = new List<StorageProviderItem>();

            do
            {
                var listSegment = await directory.ListFilesAndDirectoriesSegmentedAsync(fileContinuationToken);
                result.AddRange(listSegment.Results.OfType<CloudFileDirectory>().Select(s => new StorageProviderItem
                {
                    Id = GetId(s.Parent, s.Name),
                    Name = s.Name,
                    Type = StorageProviderItemType.Folder,
                    ParentReferenceId = parent.Id
                }));

                result.AddRange(listSegment.Results.OfType<CloudFile>().Select(s => new StorageProviderItem
                {
                    Id = GetId(s.Parent, s.Name),
                    Name = s.Name,
                    Type = StorageProviderItemType.File,
                    ParentReferenceId = parent.Id
                }));

                fileContinuationToken = listSegment.ContinuationToken;
            } while (fileContinuationToken != null);

            return result;
        }

        public Task<IEnumerable<StorageProviderItem>> GetChildrenByParentPath(string path)
        {
            return this.GetChildrenByParentItem(GetItem(path));
        }

        #endregion

        // For connection testing
        public static async Task<bool> TestShare(StorageCredentials credentials, string containerName)
        {
            var provider = new AzureFileStorageProvider();

            var fileShare = await provider.GetShare(credentials, containerName);
            var rootDir = fileShare.GetRootDirectoryReference();
            await rootDir.ListFilesAndDirectoriesSegmentedAsync(null);

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

        private async Task<CloudFile> GetFile(AccountConfiguration account, string path)
        {
            var fileShare = await GetShare(account);

            // Get specific file
            var rootDir = fileShare.GetRootDirectoryReference();
            var directory = rootDir.GetDirectoryReference(path);
            var file = directory.GetFileReference(path);

            return file;
        }

        private async Task<CloudFileShare> GetShare(AccountConfiguration account)
        {
            var shareName = "";
            if (account.AdditionalSettings != null && account.AdditionalSettings.ContainsKey("AzureItemName"))
            {
                shareName = account.AdditionalSettings["AzureItemName"];
            }

            var fileShare = await GetShare(new StorageCredentials(account.Name, account.Secret), shareName);

            return fileShare;
        }

        private async Task<CloudFileShare> GetShare(StorageCredentials credentials, string shareName)
        {
            if (credentials == null) throw new ArgumentNullException("credentials");
            if (shareName == null) throw new ArgumentNullException("shareName");

            var storageAccount = new CloudStorageAccount(credentials, true);
            var cloudFileClient = storageAccount.CreateCloudFileClient();
            var fileShare = cloudFileClient.GetShareReference(shareName);

            if (await fileShare.CreateIfNotExistsAsync())
            {
                SharedAccessFilePolicy sharedPolicy = new SharedAccessFilePolicy()
                {
                    Permissions = SharedAccessFilePermissions.Read | SharedAccessFilePermissions.Write
                };

                var permissions = await fileShare.GetPermissionsAsync();
                permissions.SharedAccessPolicies.Add("keePassSharePolicy" + DateTime.UtcNow.Ticks, sharedPolicy);
                await fileShare.SetPermissionsAsync(permissions);
            }

            return fileShare;
        }

        private string GetId(CloudFileDirectory parent, string name)
        {
            var result = name;
            while (parent != null && !string.IsNullOrEmpty(parent.Name))
            {
                result = parent.Name + "/" + result;
                parent = parent.Parent;
            }

            return result;
        }

        #endregion
    }
}