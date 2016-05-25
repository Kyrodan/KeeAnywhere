using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using KoenZomers.OneDrive.Api;
using KoenZomers.OneDrive.Api.Entities;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveStorageProvider : IStorageProvider
    {
        private readonly AccountConfiguration _account;
        private OneDriveConsumerApi _api;

        public OneDriveStorageProvider(AccountConfiguration account)
        {
            if (account == null) throw new ArgumentNullException("account");
            _account = account;
        }

        public async Task<Stream> Load(string path)
        {
            var api = await GetApi();

            var escapedpath = Uri.EscapeDataString(path);
            var stream = await api.DownloadItem(escapedpath);

            return stream;
        }


        public async Task<bool> Save(Stream stream, string path)
        {
            var api = await GetApi();

            var directory = Uri.EscapeDataString(CloudPath.GetDirectoryName(path));
            var filename = Uri.EscapeDataString(CloudPath.GetFileName(path));

            var uploadedItem = await api.UploadFileAs(stream, filename, directory);

            return uploadedItem != null;
        }

        public async Task<StorageProviderItem> GetRootItem()
        {
            var api = await GetApi();
            var odItem = await api.GetDriveRoot();

            if (odItem == null)
                return null;

            var item = CreateStorageProviderItemFromOneDriveItem(odItem);

            return item;
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");

            var api = await GetApi();
            var odChildren = await api.GetChildrenByParentItem(new OneDriveItem {Id = parent.Id});

            var children =
                odChildren.Collection.Select(odItem => CreateStorageProviderItemFromOneDriveItem(odItem)).ToArray();

            return children;
        }

        public bool IsFilenameValid(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;

            char[] invalidChars = { ':', '*', '?', '<', '>', '|', '/', '\\' };
            return filename.All(c => c >= 32 && !invalidChars.Contains(c));
        }

        protected StorageProviderItem CreateStorageProviderItemFromOneDriveItem(OneDriveItem item)
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
                ParentReferenceId = item.ParentReference != null ? item.ParentReference.Id : null
            };

            return providerItem;
        }

        public async Task<OneDriveConsumerApi> GetApi()
        {
            if (_api == null)
                _api = await OneDriveHelper.GetApi(_account.Secret);

            return _api;
        }
    }
}