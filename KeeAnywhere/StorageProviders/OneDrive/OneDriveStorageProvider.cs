using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeeAnywhere.Configuration;
using KeeAnywhere.Forms;
using KeePass.UI;
using KeePassLib.Utility;
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

        public async Task<bool> Delete(string path)
        {
            var api = await GetApi();
            var item = await api.GetItem(path);
            if (item == null) return true;

            var isOk = await api.Delete(path);

            return isOk;
        }

        public async Task<Stream> Load(string path)
        {
            var api = await GetApi();

            var tempFilename = Path.GetTempFileName();

            // Workaround due to Bug #2 in OneAdriveApi.DownloadItemAndSave(string path, string filename)
            var item = await api.GetItem(path);
            if (item == null) return null;

            var isOk = await api.DownloadItemAndSaveAs(item, tempFilename);

            if (!isOk)
            {
                File.Delete(tempFilename);
                throw new FileNotFoundException("OneDrive: File not found", path);
            }

            var content = File.ReadAllBytes(tempFilename);
            File.Delete(tempFilename);
            var stream = new MemoryStream(content, false);

            return stream;
        }


        public async Task<bool> Save(MemoryStream stream, string path)
        {
            var api = await GetApi();

            var tempFilename = Path.GetTempFileName();
            var bytes = stream.ToArray();
            File.WriteAllBytes(tempFilename, bytes);

            var directory = Path.GetDirectoryName(path);
            var filename = Path.GetFileName(path);

            var uploadedItem = await api.UploadFileAs(tempFilename, filename, directory);

            File.Delete(tempFilename);

            return uploadedItem != null;
        }

        public Task<bool> Move(string pathFrom, string pathTo)
        {
            throw new NotImplementedException("OneDrive: Move-Operation not implemented");
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
            var odChildren = await api.GetChildrenByParentItem(new OneDriveItem() {Id = parent.Id});

            var children = odChildren.Collection.Select(odItem => CreateStorageProviderItemFromOneDriveItem(odItem)).ToArray();

            return children;
        }

        protected StorageProviderItem CreateStorageProviderItemFromOneDriveItem(OneDriveItem item)
        {
            var providerItem = new StorageProviderItem
            {
                Type = item.IsFolder() ? StorageProviderItemType.Folder : (item.IsFile() ? StorageProviderItemType.File : StorageProviderItemType.Unknown),
                Id = item.Id,
                Name = item.Name,
                LastModifiedDateTime = item.LastModifiedDateTime,
                ParentReferenceId = item.ParentReference != null ? item.ParentReference.Id : null,
            };

            return providerItem;
        }

        public async Task<OneDriveConsumerApi> GetApi()
        {
            if (_api == null)
                _api = await OneDriveHelper.GetApi(_account.RefreshToken);

            return _api;
        }
    }
}