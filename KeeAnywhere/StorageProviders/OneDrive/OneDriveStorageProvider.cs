using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using Microsoft.OneDrive.Sdk;

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


        public async Task<bool> Save(Stream stream, string path)
        {
            var api = await OneDriveHelper.GetApi(_account);

            var escapedpath = Uri.EscapeDataString(path);

            var uploadedItem = await api.Drive
                        .Root
                        .ItemWithPath(escapedpath)
                        .Content
                        .Request()
                        .PutAsync<Item>(stream);

            return uploadedItem != null;
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

        public bool IsFilenameValid(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;

            char[] invalidChars = { ':', '*', '?', '<', '>', '|', '/', '\\' };
            return filename.All(c => c >= 32 && !invalidChars.Contains(c));
        }

        protected StorageProviderItem CreateStorageProviderItemFromOneDriveItem(Item item)
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