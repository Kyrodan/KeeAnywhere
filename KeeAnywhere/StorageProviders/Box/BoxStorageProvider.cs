using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Box.V2;
using Box.V2.Models;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.Box
{
    public class BoxStorageProvider : IStorageProvider
    {
        private readonly AccountConfiguration _account;
        private BoxClient _api;

        public BoxStorageProvider(AccountConfiguration account)
        {
            _account = account;
        }

        public async Task<Stream> Load(string path)
        {
            var api = await GetApi();

            var item = await api.GetFileByPath(path);
            if (item == null)
                return null;

            var stream = await api.FilesManager.DownloadStreamAsync(item.Id);

            return stream;
        }

        public async Task Save(Stream stream, string path)
        {
            var api = await GetApi();
            BoxFile item;

            var file = await api.GetFileByPath(path);
            if (file != null)
            {
                item = await api.FilesManager.UploadNewVersionAsync(file.Name, file.Id, stream, file.ETag);
            }
            else // not found: creating new
            {

                var folderName = CloudPath.GetDirectoryName(path);
                var fileName = CloudPath.GetFileName(path);

                var folder = await api.GetFileByPath(folderName);
                if (folder == null)
                    throw new InvalidOperationException(string.Format("Folder does not exist: {0}", folderName));

                var request = new BoxFileRequest()
                {
                    Name = fileName,
                    Parent = new BoxRequestEntity { Id = folder.Id },
                };

                item = await api.FilesManager.UploadAsync(request, stream);

            }

            if (item == null)
                throw new InvalidOperationException("Save to Box failed.");
        }

        public async Task Copy(string sourcePath, string destPath)
        {
            var api = await GetApi();

            var item = await api.GetFileByPath(sourcePath);
            if (item == null)
                throw new FileNotFoundException("Box: File not found.", sourcePath);

            var destFilename = CloudPath.GetFileName(destPath);
            var destFolder = CloudPath.GetDirectoryName(destPath);

            var destParent = await api.GetFileByPath(destFolder);
            if (destParent == null)
                throw new FileNotFoundException("Box: File not found.", destFolder);

            var request = new BoxFileRequest
            {
                Id = item.Id,
                Parent = new BoxFileRequest {Id = destParent.Id},
                Name = destFilename,
            };

            var result = await api.FilesManager.CopyAsync(request);

            if (result == null)
                throw new InvalidOperationException("Box: Copy failed.");

        }

        public async Task Delete(string path)
        {
            var api = await GetApi();

            var item = await api.GetFileByPath(path);
            if (item == null)
                throw new FileNotFoundException("Box: File not found for delete.", path);

            var isOk = await api.FilesManager.DeleteAsync(item.Id);

            if (!isOk)
                throw new InvalidOperationException("Box: Delete failed.");
        }


        public async Task<StorageProviderItem> GetRootItem()
        {
            return new StorageProviderItem
            {
                Id = "0",
                Name = "/",
                Type = StorageProviderItemType.Folder
            };
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent)
        {
            var api = await GetApi();

            var folderItems = await api.FoldersManager.GetFolderItemsAsync(parent.Id, BoxHelper.Limit);

            var items = folderItems.Entries.Select(_ => new StorageProviderItem()
            {
                Id = _.Id,
                Name = _.Name,
                Type = _.Type == "folder" ? StorageProviderItemType.Folder : (_.Type == "file" ? StorageProviderItemType.File : StorageProviderItemType.Unknown),
                LastModifiedDateTime = _.ModifiedAt,
                ParentReferenceId = parent.Id,
            });

            return items.ToArray();
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentPath(string path)
        {
            var api = await GetApi();

            var parent = await api.GetFileByPath(path);
            if (parent == null)
                throw new FileNotFoundException("Box: Path not found.", path);

            return await GetChildrenByParentItem(new StorageProviderItem {Id = parent.Id});
        }

        public bool IsFilenameValid(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;
            if (filename == ".") return false;
            if (filename == "..") return false;
            if (filename.StartsWith(" ") || filename.EndsWith(" ")) return false;

            char[] invalidChars = { '/', '\\', '"' };
            return filename.All(c => c >= 32 && !invalidChars.Contains(c));
        }

        private async Task<BoxClient> GetApi()
        {
            if (_api == null)
                _api = await BoxHelper.GetClient(_account);

            return _api;
        }




    }
}