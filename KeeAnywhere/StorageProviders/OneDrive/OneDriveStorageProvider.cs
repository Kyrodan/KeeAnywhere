using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Drives.Item.Items.Item.Copy;
using Microsoft.Graph.Models;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveStorageProvider : IStorageProvider
    {
        private readonly AccountConfiguration _account;
        private readonly GraphServiceClient _api;

        public OneDriveStorageProvider(AccountConfiguration account)
        {
            if (account == null) throw new ArgumentNullException("account");
            _account = account;
            _api = OneDriveHelper.GetApi(account);
        }

        public async Task<Stream> Load(string path)
        {
            var stream = await (await _api.DriveItemFromPathAsync(path))
                                    .Content
                                    .GetAsync();

            return stream;
        }


        public async Task Save(Stream stream, string path)
        {
            var uploadedItem = await (await _api.DriveItemFromPathAsync(path))
                        .Content
                        .PutAsync(stream);

            if (uploadedItem == null)
                throw new InvalidOperationException("Save to OneDrive failed.");

        }

        public async Task Copy(string sourcePath, string destPath)
        {
            var destFilename = CloudPath.GetFileName(destPath);
            var destItem = await (await _api.DriveItemFromPathAsync(destPath)).GetAsync();
            if (destItem == null)
                throw new FileNotFoundException("OneDrive: Folder not found.", destPath);

            var body = new CopyPostRequestBody
            {
                Name = destFilename,
                ParentReference = new ItemReference { Id = destItem.Id }
            };
            
            await (await _api.DriveItemFromPathAsync(sourcePath))
                .Copy.PostAsync(body);
        }

        public async Task Delete(string path)
        {
            await (await _api.DriveItemFromPathAsync(path)).DeleteAsync();
        }

        public async Task<StorageProviderItem> GetRootItem()
        {
            var rootItem = await _api.Me.Drive.GetAsync();
            var odItem = await _api.Drives[rootItem.Id].Root.GetAsync();

            if (odItem == null)
                return null;

            var item = new StorageProviderItem
            {
                Type = StorageProviderItemType.Folder,
                Id = MakeStorageProviderItemId(odItem),
                Name = odItem.Name,
                LastModifiedDateTime = odItem.LastModifiedDateTime,
                ParentReferenceId = null
            };
            return item;
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");

            var odChildren = await _api.DriveItemFromStorageProviderItemId(parent.Id).Children.GetAsync();

            var children =
                odChildren.Value.Select(odItem => CreateStorageProviderItemFromOneDriveItem(odItem)).ToArray();

            return children;
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentPath(string path)
        {
            var odChildren = await (await _api.DriveItemFromPathAsync(path)).Children.GetAsync();
            var children =
                odChildren.Value.Select(odItem => CreateStorageProviderItemFromOneDriveItem(odItem)).ToArray();

            return children;
        }

        public bool IsFilenameValid(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;

            char[] invalidChars = { ':', '*', '?', '<', '>', '|', '/', '\\' };
            return filename.All(c => c >= 32 && !invalidChars.Contains(c));
        }

        protected static StorageProviderItem CreateStorageProviderItemFromOneDriveItem(DriveItem item)
        {
            var providerItem = new StorageProviderItem
            {
                Type = DetermineStorageProviderItemType(item),
                Id = MakeStorageProviderItemId(item),
                Name = item.Name,
                LastModifiedDateTime = item.LastModifiedDateTime,
                ParentReferenceId = MakeStorageProviderItemParentId(item)
            };

            return providerItem;
        }

        public static StorageProviderItemType DetermineStorageProviderItemType(DriveItem item)
        {
            if (item.RemoteItem == null)
            {
                if (item.Folder != null) return StorageProviderItemType.Folder;
                if (item.File != null) return StorageProviderItemType.File;
            }
            else
            {
                if (item.RemoteItem.Folder != null) return StorageProviderItemType.Folder;
                if (item.RemoteItem.File != null) return StorageProviderItemType.File;
            }
            return StorageProviderItemType.Unknown;
        }

        private static string MakeStorageProviderItemId(DriveItem item)
        {
            if (item == null) throw new ArgumentNullException("item");

            // If a user "adds" a folder was shared with them to their
            // OneDrive, they effectively get a symbolic link in their
            // default root folder which we will see as an item with
            // a non-null RemoteItem. To access its contents, we need
            // to use its real drive identifier, and the item identifier
            // for that context, both of which we get via RemoteItem.
            // We store these in the item's ID, instead of information
            // about the symbolic link itself, so that when we're 
            // given its StorageProviderItem later, we can retrieve
            // the folder content without doing another lookup.
            //
            // This works because, as of this writing, the only
            // IStorageProvider method that takes a StorageProviderItem
            // as an argument is the one requesting its children. If
            // that changes, then we will need to store information about
            // both the symbolic link and the remote item.
            if (item.RemoteItem == null)
                return MakeStorageProviderItemId(item.ParentReference.DriveId, item.Id);
            return MakeStorageProviderItemId(item.RemoteItem.ParentReference.DriveId, item.RemoteItem.Id);
        }

        private static string MakeStorageProviderItemParentId(DriveItem item)
        {
            if (item == null) throw new ArgumentNullException("item");
            // If RemoteItem is not null, this is a top-level access point to 
            // a shared item. The parent ID in this case should refer back to 
            // something in the user's own file space (currently, always the 
            // default drive root), so we ignore the parent information in 
            // RemoteItem.
            //
            // If this item is remote, its parent's id will be an id in the
            // remote drive. See also comments in MakeStorageProviderItemId.
            return MakeStorageProviderItemId(item.ParentReference.DriveId, item.ParentReference.Id);
        }

        public static string MakeStorageProviderItemId(string drive, string item)
        {
            // Since "remote" items are on a drive other than the default, we
            // construct item identifiers that include both the drive 
            // identifier and the actual item identifier, making the drive 
            // selection explicit.
            if (string.IsNullOrEmpty(drive)) throw new ArgumentOutOfRangeException("drive");
            if (string.IsNullOrEmpty(item)) throw new ArgumentOutOfRangeException("item");
            return drive + "/" + item;
        }

    }
}