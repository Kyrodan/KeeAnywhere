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
                    _api = DropboxHelper.GetApi(account.Type == StorageType.DropboxRestricted, account.Secret);

                return _api;
            }
        }

        public async Task<Stream> Load(string path)
        {
            path = RootPath(path);

            var response = await Api.Files.DownloadAsync(path);
            var stream = await response.GetContentAsStreamAsync();

            return stream;
        }

        public async Task Save(Stream stream, string path)
        {
            path = RootPath(path);

            var result = await Api.Files.UploadAsync(path, WriteMode.Overwrite.Instance, body: stream);

            if (!result.IsFile)
                throw new InvalidOperationException("Save to Dropbox failed.");
        }

        public async Task Copy(string sourcePath, string destPath)
        {
            sourcePath = RootPath(sourcePath);
            destPath = RootPath(destPath);

            var response = await Api.Files.CopyV2Async(sourcePath, destPath);

            if (response == null)
                throw new InvalidOperationException("Dropbox: Copy failed.");
        }

        public async Task Delete(string path)
        {
            path = RootPath(path);
            var response = await Api.Files.DeleteV2Async(path);

            if (response == null)
                throw new InvalidOperationException("Dropbox: Delete failed.");
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

            var dbxItems = await Api.Files.ListFolderAsync(parent.Id);
            var items = dbxItems.Entries.Select(_ => CreateStorageProviderItem(parent, _));

            while (dbxItems.HasMore)
            {
                dbxItems = await Api.Files.ListFolderContinueAsync(dbxItems.Cursor);
                items = items.Concat(dbxItems.Entries.Select(_ => CreateStorageProviderItem(parent, _)));
            }

            return items.ToArray();
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentPath(string path)
        {
            return await GetChildrenByParentItem(new StorageProviderItem { Id = RootPath(path) });
        }

        public bool IsFilenameValid(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;

            char[] invalidChars = { '<', '>', '/', '\\', ':', '*', '?', '"', '|' };
            return filename.All(c => c >= 32 && !invalidChars.Contains(c));
        }

        private static string RootPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = "";
            }
            else if (path[0] != CloudPath.DirectorySeparatorChar)
            {
                path = CloudPath.DirectorySeparatorChar + path;
            }

            return path;
        }

        private static StorageProviderItem CreateStorageProviderItem(StorageProviderItem parent, Metadata item)
        {
            var result = new StorageProviderItem
            {
                Name = item.Name,
                Id = item.PathLower,
                //Id = CloudPath.Combine(parent.Id, item.PathLower),
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