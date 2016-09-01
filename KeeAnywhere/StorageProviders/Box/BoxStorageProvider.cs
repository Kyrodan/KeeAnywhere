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

        public async Task<bool> Save(Stream stream, string path)
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

            return item != null;
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