using Microsoft.Graph;
using Microsoft.Graph.Drives.Item.Items.Item;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public static class OneDriveApiExtensions
    {

        /// <summary>
        /// Configures a Graph API request for an object identified by the
        /// item identifier stored in a StorageProviderItem object.
        /// </summary>
        /// <param name="api">A Graph API request builder.</param>
        /// <param name="itemId">
        /// The item ID from a StorageProviderItem object that was created
        /// by the OneDrive storage provider.
        /// </param>
        /// <returns></returns>
        public static DriveItemItemRequestBuilder DriveItemFromStorageProviderItemId(this GraphServiceClient api, string itemId)
        {
            // ID must include a drive id prefix. 
            // See also OneDriveStorageProvider.MakeStorageProviderItemId.
            var idParts = itemId.Split('/');
            if (idParts.Length != 2) throw new ArgumentOutOfRangeException("itemId");
            return api.Drives[idParts[0]].Items[idParts[1]];
        }

        /// <summary>
        /// Configures a Graph API request for a OneDrive drive based on a 
        /// path from a user's default drive. Accommodates top-level folders 
        /// which are links to remote, shared items. 
        /// </summary>
        /// <param name="api">A Graph API request builder.</param>
        /// <param name="path">
        /// A URI path relative to the user's default drive.
        /// </param>
        /// <returns>
        /// A task that yields a drive item request builder.
        /// </returns>
        /// <remarks>
        /// An extra Web request has to be made to determine if the top 
        /// folder is remote or local. That's why this method is async.
        /// </remarks>
        public async static Task<DriveItemItemRequestBuilder> DriveItemFromPathAsync(this GraphServiceClient api, string path)
        {
            // The top folder could be a shared folder, in which case it's 
            // on a different drive than the default. The path will use the
            // name of the link in the user's root, which may be different
            // from its actual (remote) name.
            if (string.IsNullOrEmpty(path)) throw new ArgumentOutOfRangeException("path");
            var parts = path.Split('/');
            var rootItem = await api.Me.Drive.GetAsync();

            if (parts.Length == 1)
            {
                return api.Drives[rootItem.Id].Root.ItemWithPath(parts[0]);
            }

            var topFolder = await api.Drives[rootItem.Id].Root.ItemWithPath(Uri.EscapeDataString(parts[0])).GetAsync();
            var driveId = topFolder.RemoteItem == null ? topFolder.ParentReference.DriveId : topFolder.RemoteItem.ParentReference.DriveId;
            var topFolderId = topFolder.RemoteItem == null ? topFolder.Id : topFolder.RemoteItem.Id;
            // The top folder's apparent name can be different from its 
            // actual name, so don't use the name as part of the path at all.
            // We have the id, so navigate from there instead.
            return api.Drives[driveId].Items[topFolderId].ItemWithPath(Uri.EscapeDataString(string.Join("/",parts.Skip(1))));
        }

    }
}