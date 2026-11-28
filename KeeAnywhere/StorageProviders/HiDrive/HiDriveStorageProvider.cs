using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using Kyrodan.HiDrive;
using Kyrodan.HiDrive.Models;
using Kyrodan.HiDrive.Requests;

namespace KeeAnywhere.StorageProviders.HiDrive
{
    public class HiDriveStorageProvider : IStorageProvider
    {
        private AccountConfiguration account;
        private IHiDriveClient _api;
        private string _homeId;

        public HiDriveStorageProvider(AccountConfiguration account)
        {
            this.account = account;
        }

        // #324: the file dialog starts at the HiDrive root, so paths chosen there start with one of its
        // folders ("users/<name>/..." = Personal, "public/..." = Common) and are absolute. Older paths
        // (up to 2.3) are relative to the home folder and keep working.
        // shortcut: a path in the home folder starting with a folder named "users" or "public" is taken
        // as absolute - reopen such a database via the file dialog once.
        private static readonly string[] RootFolders = { "users", "public" };

        private class Location
        {
            public string Path;
            public string Pid;
        }

        private async Task<Location> GetLocation(string path)
        {
            var firstPart = path.Split('/')[0];
            if (RootFolders.Contains(firstPart, StringComparer.OrdinalIgnoreCase))
                return new Location { Path = "/" + path, Pid = null };

            return new Location { Path = string.IsNullOrEmpty(path) ? null : path, Pid = await GetHomeId() };
        }

        public async Task<Stream> Load(string path)
        {
            var api = await GetApi();
            var location = await GetLocation(path);
            var stream = await api.File.Download(location.Path, location.Pid).ExecuteAsync();

            return stream;
        }

        public async Task Save(Stream stream, string path)
        {
            var api = await GetApi();
            var folder = await GetLocation(CloudPath.GetDirectoryName(path));
            var filename = CloudPath.GetFileName(path);

            var item = await api.File.Upload(filename, folder.Path, folder.Pid, UploadMode.CreateOrUpdate).ExecuteAsync(stream);

            if (item == null)
                throw new InvalidOperationException("HiDrive: Save failed.");
        }

        public async Task Copy(string sourcePath, string destPath)
        {
            var api = await GetApi();
            var source = await GetLocation(sourcePath);
            var dest = await GetLocation(destPath);

            await api.File.Copy(source.Path, source.Pid, dest.Path, dest.Pid).ExecuteAsync();
        }

        public async Task Delete(string path)
        {
            var api = await GetApi();
            var location = await GetLocation(path);

            await api.File.Delete(location.Path, location.Pid).ExecuteAsync();
        }

        public async Task<StorageProviderItem> GetRootItem()
        {
            var api = await GetApi();

            DirectoryItem root = null;
            try
            {
                root = await api.Directory.Get("/", null, new[] { DirectoryMember.None }, new[] { DirectoryBaseItem.Fields.Id }).ExecuteAsync();
            }
            catch (Exception)
            {
                // e.g. no read permission for the root: fall back to the home folder (paths relative to it)
            }

            if (root != null)
            {
                return new StorageProviderItem
                {
                    Id = root.Id,
                    Name = "/",
                    Type = StorageProviderItemType.Folder,
                };
            }

            var user = await api.User.Me.Get(new[] { User.Fields.HomeId, User.Fields.Home }).ExecuteAsync();

            return new StorageProviderItem
            {
                Id = user.HomeId,
                Name = user.Home,
                Type = StorageProviderItemType.Folder,
            };
        }

        // Folders only need to be readable (e.g. "/" and "/users" aren't writable), files must be writable to save a database
        private static bool IsListed(DirectoryChildItem item)
        {
            return item.IsReadable.Value && (item.Type == "dir" || item.IsWritable.Value);
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent)
        {
            var api = await GetApi();

            var fields = new[]
            {
                DirectoryBaseItem.Fields.Members_Path, DirectoryBaseItem.Fields.Members_Name, DirectoryBaseItem.Fields.Members_Id, DirectoryBaseItem.Fields.Members_Type, DirectoryBaseItem.Fields.Members_IsReadable, DirectoryBaseItem.Fields.Members_IsWritable, DirectoryBaseItem.Fields.Members_ModiefiedDateTime
            };

            var dir = await api
                .Directory
                .Get(null, parent.Id, new [] {DirectoryMember.Directory, DirectoryMember.File }, fields)
                .ExecuteAsync();

            var items = dir.Members.Where(IsListed).Select(_ => new StorageProviderItem
            {
                Id = _.Id,
                Name = _.Name,
                Type =
                    _.Type == "file"
                        ? StorageProviderItemType.File
                        : (_.Type == "dir" ? StorageProviderItemType.Folder : StorageProviderItemType.Unknown),
                ParentReferenceId = parent.Id,
                LastModifiedDateTime = _.ModifiedDateTime,
            });

            return items.ToArray();
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentPath(string path)
        {
            var api = await GetApi();
            var location = await GetLocation(path);
            var pid = location.Pid;

            var fields = new[]
            {
                DirectoryBaseItem.Fields.Members_Path, DirectoryBaseItem.Fields.Members_Name, DirectoryBaseItem.Fields.Members_Id, DirectoryBaseItem.Fields.Members_Type, DirectoryBaseItem.Fields.Members_IsReadable, DirectoryBaseItem.Fields.Members_IsWritable, DirectoryBaseItem.Fields.Members_ModiefiedDateTime
            };

            var dir = await api
                .Directory
                .Get(location.Path, pid, new[] { DirectoryMember.Directory, DirectoryMember.File }, fields)
                .ExecuteAsync();

            var items = dir.Members.Where(IsListed).Select(_ => new StorageProviderItem
            {
                Id = _.Id,
                Name = _.Name,
                Type =
                    _.Type == "file"
                        ? StorageProviderItemType.File
                        : (_.Type == "dir" ? StorageProviderItemType.Folder : StorageProviderItemType.Unknown),
                ParentReferenceId = pid,
                LastModifiedDateTime = _.ModifiedDateTime,
            });

            return items.ToArray();

        }

        public bool IsFilenameValid(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;
            if (filename == ".") return false;
            if (filename == "..") return false;
            if (filename.StartsWith(" ") || filename.EndsWith(" ")) return false;

            char[] invalidChars = { '/', '\\' };
            return filename.All(c => c >= 32 && !invalidChars.Contains(c));
        }

        private async Task<IHiDriveClient> GetApi()
        {
            if (_api == null)
                _api = await HiDriveHelper.GetClient(account);

            return _api;
        }

        private async Task<string> GetHomeId()
        {
            if (_homeId == null)
            {
                var api = await GetApi();
                var user = await api.User.Me.Get(new[] { User.Fields.HomeId }).ExecuteAsync();
                _homeId = user.HomeId;
            }

            return _homeId;
        }

    }
}