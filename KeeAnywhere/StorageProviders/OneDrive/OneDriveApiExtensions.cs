using Microsoft.Graph;
using Microsoft.Graph.Drives.Item.Items.Item;
using Microsoft.Graph.Models;
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
        /// Resolves a path under the user's default drive (or a remote
        /// shared mount) to an Items[id] request builder.
        /// </summary>
        /// <remarks>
        /// Personal OneDrive does not reliably honor path-based item URLs
        /// for content operations: the /:/path:/ form 404s on download, and
        /// the itemWithPath(path='...') function form URL-encodes '/' as
        /// %2F which Graph treats as a literal name. We walk the path one
        /// segment at a time via Children listings so callers always get a
        /// /drives/{drive}/items/{id} URL.
        /// </remarks>
        public async static Task<DriveItemItemRequestBuilder> DriveItemFromPathAsync(this GraphServiceClient api, string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentOutOfRangeException("path");
            var parts = path.Split('/');
            var drive = await api.Me.Drive.GetAsync();

            var driveId = drive.Id;
            var currentId = (await api.Drives[driveId].Root.GetAsync()).Id;

            foreach (var segment in parts)
            {
                var match = await FindChildAsync(api, driveId, currentId, segment);
                if (match == null)
                    throw new System.IO.FileNotFoundException("OneDrive: '" + segment + "' not found under '" + path + "'");

                if (match.RemoteItem != null)
                {
                    driveId = match.RemoteItem.ParentReference.DriveId;
                    currentId = match.RemoteItem.Id;
                }
                else
                {
                    currentId = match.Id;
                }
            }

            return api.Drives[driveId].Items[currentId];
        }

        // Microsoft Graph paginates Children at 200 items per page by default.
        // Walk pages until we find the named child or exhaust the listing.
        private static async Task<DriveItem> FindChildAsync(GraphServiceClient api, string driveId, string parentId, string name)
        {
            var page = await api.Drives[driveId].Items[parentId].Children.GetAsync();
            while (page != null)
            {
                var match = page.Value.FirstOrDefault(c => c.Name == name);
                if (match != null) return match;
                if (string.IsNullOrEmpty(page.OdataNextLink)) return null;
                page = await api.Drives[driveId].Items[parentId].Children
                    .WithUrl(page.OdataNextLink)
                    .GetAsync();
            }
            return null;
        }

    }
}