using Microsoft.Graph;
using Microsoft.Graph.Drives.Item.Items.Item;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public static class OneDriveApiExtensions
    {
        // Path resolution per account (#508): the own drive id never changes; a top folder (drive id and
        // item id, also of shared folders) is kept for a few minutes - enough for open, save and verify,
        // short enough that a renamed or replaced folder is found again. Bound to the GraphServiceClient,
        // which OneDriveHelper caches per account and drops on re-authorization.
        private static readonly TimeSpan TopFolderLifetime = TimeSpan.FromMinutes(5);

        private class TopFolder
        {
            public string DriveId;
            public string ItemId;
            public DateTime Expires;
        }

        private class PathCache
        {
            public string DriveId;
            public readonly Dictionary<string, TopFolder> TopFolders = new Dictionary<string, TopFolder>(StringComparer.OrdinalIgnoreCase);
            public bool AppFolderOnly;
            public string AppFolderId;
        }

        private static readonly ConditionalWeakTable<GraphServiceClient, PathCache> PathCaches = new ConditionalWeakTable<GraphServiceClient, PathCache>();

        // OneDrive (restricted) (#391): the client only has access to the app folder Apps/KeeAnywhere
        // (special folder "approot"), so all paths are relative to it.
        public static void UseAppFolderAsRoot(this GraphServiceClient api)
        {
            PathCaches.GetOrCreateValue(api).AppFolderOnly = true;
        }

        public static async Task<DriveItem> GetRootFolderAsync(this GraphServiceClient api)
        {
            var driveId = await api.GetDefaultDriveIdAsync();
            return PathCaches.GetOrCreateValue(api).AppFolderOnly
                ? await api.Drives[driveId].Special["approot"].GetAsync()
                : await api.Drives[driveId].Root.GetAsync();
        }

        public static async Task<string> GetDefaultDriveIdAsync(this GraphServiceClient api)
        {
            var cache = PathCaches.GetOrCreateValue(api);
            if (cache.DriveId == null)
                cache.DriveId = (await api.Me.Drive.GetAsync()).Id;

            return cache.DriveId;
        }

        /// <summary>
        /// Runs a request for the item at the given path. The cached top folder may be outdated
        /// (e.g. renamed or removed): on 404 it's resolved again and the request is retried once.
        /// </summary>
        public static async Task<T> RequestByPathAsync<T>(this GraphServiceClient api, string path, Func<CustomDriveItemItemRequestBuilder, Task<T>> request)
        {
            try
            {
                return await request(await api.DriveItemFromPathAsync(path));
            }
            catch (ApiException ex)
            {
                if (ex.ResponseStatusCode != 404 || !RemoveCachedTopFolder(api, path)) throw;
            }

            return await request(await api.DriveItemFromPathAsync(path));
        }

        private static bool RemoveCachedTopFolder(GraphServiceClient api, string path)
        {
            var parts = path.Split('/');
            if (parts.Length == 1) return false;

            var cache = PathCaches.GetOrCreateValue(api);
            lock (cache)
            {
                return cache.TopFolders.Remove(parts[0]);
            }
        }

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
        // The base builder drops the file path when accessing Content, Children or Copy.
        public async static Task<CustomDriveItemItemRequestBuilder> DriveItemFromPathAsync(this GraphServiceClient api, string path)
        {
            // The top folder could be a shared folder, in which case it's 
            // on a different drive than the default. The path will use the
            // name of the link in the user's root, which may be different
            // from its actual (remote) name.
            if (string.IsNullOrEmpty(path)) throw new ArgumentOutOfRangeException("path");
            var parts = path.Split('/');
            var defaultDriveId = await api.GetDefaultDriveIdAsync();
            var cache = PathCaches.GetOrCreateValue(api);

            if (cache.AppFolderOnly)
            {
                // no shared folders in the app folder, the app folder's id never changes
                if (cache.AppFolderId == null)
                    cache.AppFolderId = (await api.Drives[defaultDriveId].Special["approot"].GetAsync()).Id;

                return api.Drives[defaultDriveId].Items[cache.AppFolderId].ItemWithPath(Uri.EscapeDataString(path));
            }

            if (parts.Length == 1)
            {
                return api.Drives[defaultDriveId].Root.ItemWithPath(parts[0]);
            }

            TopFolder top;
            lock (cache)
            {
                cache.TopFolders.TryGetValue(parts[0], out top);
            }

            if (top == null || top.Expires < DateTime.UtcNow)
            {
                var topFolder = await api.Drives[defaultDriveId].Root.ItemWithPath(Uri.EscapeDataString(parts[0])).GetAsync();
                top = new TopFolder
                {
                    DriveId = topFolder.RemoteItem == null ? topFolder.ParentReference.DriveId : topFolder.RemoteItem.ParentReference.DriveId,
                    ItemId = topFolder.RemoteItem == null ? topFolder.Id : topFolder.RemoteItem.Id,
                    Expires = DateTime.UtcNow + TopFolderLifetime
                };
                lock (cache)
                {
                    cache.TopFolders[parts[0]] = top;
                }
            }

            // The top folder's apparent name can be different from its
            // actual name, so don't use the name as part of the path at all.
            // We have the id, so navigate from there instead.
            return api.Drives[top.DriveId].Items[top.ItemId].ItemWithPath(Uri.EscapeDataString(string.Join("/",parts.Skip(1))));
        }

    }
}