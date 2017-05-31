using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using Microsoft.Graph;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveStorageProvider : IStorageProvider
    {
        private readonly AccountConfiguration _account;

        public OneDriveStorageProvider(AccountConfiguration account)
        {
            if (account == null) throw new ArgumentNullException("account");
            _account = account;
        }

        public async Task<Stream> Load(string path)
        {
            var api = await OneDriveHelper.GetApi(_account);

            var escapedpath = Uri.EscapeDataString(path);
            var stream = await api.Drive
                                    .Root
                                    .ItemWithPath(escapedpath)
                                    .Content
                                    .Request()
                                    .GetAsync();

            return stream;
        }


        public async Task Save(Stream stream, string path)
        {
            var api = await OneDriveHelper.GetApi(_account);

            var escapedpath = Uri.EscapeDataString(path);

            var uploadedItem = await api.Drive
                        .Root
                        .ItemWithPath(escapedpath)
                        .Content
                        .Request()
                        .PutAsync<DriveItem>(stream);

            if (uploadedItem == null)
                throw new InvalidOperationException("Save to OneDrive failed.");

        }

        public async Task Copy(string sourcePath, string destPath)
        {
            var api = await OneDriveHelper.GetApi(_account);

            var escapedpath = Uri.EscapeDataString(sourcePath);

            var destFolder = Uri.EscapeDataString(CloudPath.GetDirectoryName(destPath));
            var destFilename = CloudPath.GetFileName(destPath);
            var destItem = await api.Drive.Root.ItemWithPath(destFolder).Request().GetAsync();
            if (destItem == null)
                throw new FileNotFoundException("OneDrive: Folder not found.", destFolder);

            await api
                .Drive
                .Root
                .ItemWithPath(escapedpath)
                .Copy(destFilename, new ItemReference {Id = destItem.Id})
                .Request(/*new[] {new HeaderOption("Prefer", "respond-async"), }*/)
                .PostAsync();
        }

        public async Task Delete(string path)
        {
            var api = await OneDriveHelper.GetApi(_account);

            var escapedpath = Uri.EscapeDataString(path);
            await api
                    .Drive
                    .Root
                    .ItemWithPath(escapedpath)
                    .Request()
                    .DeleteAsync();

        }

        public async Task<StorageProviderItem> GetRootItem()
        {
            var api = await OneDriveHelper.GetApi(_account);
            var odItem = await api.Drive.Root.Request().GetAsync();

            if (odItem == null)
                return null;

            var item = CreateStorageProviderItemFromOneDriveItem(odItem);

            return item;
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");

            var api = await OneDriveHelper.GetApi(_account);

            var odChildren = await api.Drive.Items[parent.Id].Children.Request().GetAsync();

            var children =
                odChildren.Select(odItem => CreateStorageProviderItemFromOneDriveItem(odItem)).ToArray();

            return children;
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentPath(string path)
        {
            var api = await OneDriveHelper.GetApi(_account);

            var odChildren = await api.Drive.Root.ItemWithPath(path).Children.Request().GetAsync();

            var children =
                odChildren.Select(odItem => CreateStorageProviderItemFromOneDriveItem(odItem)).ToArray();

            return children;
        }

        public bool IsFilenameValid(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;

            char[] invalidChars = { ':', '*', '?', '<', '>', '|', '/', '\\' };
            return filename.All(c => c >= 32 && !invalidChars.Contains(c));
        }

        protected StorageProviderItem CreateStorageProviderItemFromOneDriveItem(DriveItem item)
        {
            var providerItem = new StorageProviderItem
            {
                Type = 
                    item.IsFolder()
                        ? StorageProviderItemType.Folder
                        : (item.IsFile() ? StorageProviderItemType.File : StorageProviderItemType.Unknown),
                Id = item.Id,
                Name = item.Name,
                LastModifiedDateTime = item.LastModifiedDateTime,
                ParentReferenceId = item.ParentReference != null && !string.IsNullOrEmpty(item.ParentReference.Path) ? item.ParentReference.Id : null
            };

            return providerItem;
        }
    }
}