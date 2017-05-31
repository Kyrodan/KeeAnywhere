using ACD = Azi.Amazon.CloudDrive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azi.Amazon.CloudDrive.JsonObjects;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.AmazonDrive
{
    public class AmazonDriveStorageProvider : IStorageProvider
    {
        private readonly AccountConfiguration account;
        private ACD.IAmazonDrive _api;

        public AmazonDriveStorageProvider(AccountConfiguration account)
        {
            this.account = account;
        }

        protected async Task<ACD.IAmazonDrive> GetApi()
        {
            if (_api == null)
            {
                _api = AmazonDriveHelper.GetApi();
                var isOk = await _api.AuthenticationByTokens(null, this.account.Secret, DateTime.Now);

                if (!isOk)
                    throw new InvalidOperationException("Authentication to Amazon Drive failed.");
            }

            return _api;
        }

        public async Task<Stream> Load(string path)
        {
            var api = await GetApi();
            var node = await api.GetNodeByPath(path);

            if (node == null)
                return null;

            var stream = new MemoryStream();

            await api.Files.Download(node.id, stream);

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        public async Task Save(Stream stream, string path)
        {
            var api = await GetApi();

            var node = await api.GetNodeByPath(path);
            AmazonNode result;

            if (node != null)
            {
                result = await api.Files.Overwrite(node.id, () => stream);
            }
            else // not found: creating new
            {

                var folderName = CloudPath.GetDirectoryName(path);
                var fileName = CloudPath.GetFileName(path);

                var folder = await api.GetNodeByPath(folderName);
                if (folder == null)
                    throw new InvalidOperationException(string.Format("Folder does not exist: {0}", folderName));

                result = await api.Files.UploadNew(folder.id, fileName, () => stream);
            }

            if (result == null)
            {
                throw new InvalidOperationException("Save to Amazon Drive failed.");
            }
        }

        public async Task Copy(string sourcePath, string destPath)
        {
            var api = await GetApi();
            var node = await api.GetNodeByPath(sourcePath);

            var destFilename = CloudPath.GetFileName(destPath);
            var destFolder = CloudPath.GetDirectoryName(destPath);
            if (!sourcePath.StartsWith(destFolder))
                throw new InvalidOperationException("Amazon Drive: Copy failed - Dest must be in the same folder as source.");

            var result = await api.Nodes.Rename(node.id, destFilename);
            if (result == null)
                throw new InvalidOperationException("Amazon Drive: Copy failed.");
        }

        public async Task Delete(string path)
        {
            var api = await GetApi();
            var node = await api.GetNodeByPath(path);

            foreach (var parentId in node.parents)
            {
                await api.Nodes.Remove(parentId, node.id);
            }
        }

        public async Task<StorageProviderItem> GetRootItem()
        {
            return new StorageProviderItem
                {
                    Id = string.Empty,
                    Name = "Root",
                    Type = StorageProviderItemType.Folder
                };
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");

            var api = await GetApi();

            var driveItems = await api.Nodes.GetChildren(string.IsNullOrEmpty(parent.Id) ? null : parent.Id);
            var items = driveItems.Select(_ => CreateStorageProviderItem(parent, _));

            return items.ToArray();
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentPath(string path)
        {
            return await GetChildrenByParentItem(new StorageProviderItem {Id = path});
        }

        public bool IsFilenameValid(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;

            char[] invalidChars = { '<', '>', '/', '\\', ':', '*', '?', '"', '|' };
            return filename.All(c => c >= 32 && !invalidChars.Contains(c));
        }

        private static StorageProviderItem CreateStorageProviderItem(StorageProviderItem parent, AmazonNode item)
        {
            var result = new StorageProviderItem
            {
                Name = item.name,
                Id = item.id,
                ParentReferenceId = parent.Id,
                LastModifiedDateTime = item.modifiedDate
            };

            switch (item.kind)
            {
                case AmazonNodeKind.FILE:
                    result.Type = StorageProviderItemType.File;
                    break;
                case AmazonNodeKind.FOLDER:
                    result.Type = StorageProviderItemType.Folder;
                    break;
                default:
                    result.Type = StorageProviderItemType.Unknown;
                    break;
            }

            return result;
        }
    }
}