using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.Dropbox
{
    public class DropboxStorageProvider : IStorageProvider
    {
        private readonly AccountConfiguration account;
        private DropboxClient _api;

        public DropboxStorageProvider(AccountConfiguration account)
        {
            this.account = account;
        }

        protected DropboxClient Api
        {
            get
            {
                if (_api == null)
                    _api = new DropboxClient(account.Secret);

                return _api;
            }
        }

        public async Task<bool> Delete(string path)
        {
            path = RootPath(path);
            var result = await Api.Files.DeleteAsync(path);

            return result.IsDeleted;
        }

        public async Task<Stream> Load(string path)
        {
            path = RootPath(path);

            var response = await Api.Files.DownloadAsync(path);
            var stream = await response.GetContentAsStreamAsync();

            return stream;
        }

        public async Task<bool> Save(Stream stream, string path)
        {
            path = RootPath(path);

            //var ci = new CommitInfo(path);

            var result = await Api.Files.UploadAsync(path, WriteMode.Overwrite.Instance, body: stream);

            return result.IsFile;
        }

        public Task<bool> Move(string pathFrom, string pathTo)
        {
            throw new NotImplementedException();
        }

#pragma warning disable 1998
        public async Task<StorageProviderItem> GetRootItem()
#pragma warning restore 1998
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
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var dbxItems = await Api.Files.ListFolderAsync(parent.Id);
            var items = dbxItems.Entries.Select(_ => CreateStorageProviderItem(parent, _));

            return items.ToArray();
        }

        private static string RootPath(string path)
        {
            if (!path.StartsWith("/"))
                path = "/" + path;

            return path;
        }

        private static StorageProviderItem CreateStorageProviderItem(StorageProviderItem parent, Metadata item)
        {
            var result = new StorageProviderItem
            {
                Name = item.Name,
                Id = Path.Combine(parent.Id, item.PathLower),
                ParentReferenceId = parent.Id
            };

            if (item.IsFile)
            {
                result.Type = StorageProviderItemType.File;
//                result.Id = item.AsFile.Id; // Path.Combine(parent.Id, item.PathLower)
                result.LastModifiedDateTime = item.AsFile.ServerModified;
            }
            else if (item.IsFolder)
            {
                result.Type = StorageProviderItemType.Folder;
//                result.Id = item.AsFolder.Id; // Path.Combine(parent.Id, item.PathLower);
            }
            else
            {
                result.Type = StorageProviderItemType.Unknown;
            }

            return result;
        }
    }
}