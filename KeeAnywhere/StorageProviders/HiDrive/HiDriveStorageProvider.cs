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

        public async Task<Stream> Load(string path)
        {
            var api = await GetApi();
            var pid = await GetHomeId();
            var stream = await api.File.Download(path, pid).ExecuteAsync();

            return stream;
        }

        public async Task Save(Stream stream, string path)
        {
            var api = await GetApi();
            var pid = await GetHomeId();

            var pathname = CloudPath.GetDirectoryName(path);
            var filename = CloudPath.GetFileName(path);

            var item = await api.File.Upload(filename, String.IsNullOrEmpty(pathname) ? null : pathname, pid, UploadMode.CreateOrUpdate).ExecuteAsync(stream);

            if (item == null)
                throw new InvalidOperationException("HiDrive: Save failed.");
        }

        public async Task Copy(string sourcePath, string destPath)
        {
            var api = await GetApi();
            var pid = await GetHomeId();

            await api.File.Copy(sourcePath, pid, destPath, pid).ExecuteAsync();
        }

        public async Task Delete(string path)
        {
            var api = await GetApi();
            var pid = await GetHomeId();

            await api.File.Delete(path, pid).ExecuteAsync();
        }

        public async Task<StorageProviderItem> GetRootItem()
        {
            var api = await GetApi();

            var user = await api.User.Me.Get(new[] {User.Fields.HomeId, User.Fields.Home }).ExecuteAsync();

            return new StorageProviderItem
            {
                Id = user.HomeId,
                Name = user.Home,
                Type = StorageProviderItemType.Folder,
            };
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

            var items = dir.Members.Where(_ => _.IsReadable.Value && _.IsWritable.Value).Select(_ => new StorageProviderItem
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
            var pid = await GetHomeId();

            var fields = new[]
            {
                DirectoryBaseItem.Fields.Members_Path, DirectoryBaseItem.Fields.Members_Name, DirectoryBaseItem.Fields.Members_Id, DirectoryBaseItem.Fields.Members_Type, DirectoryBaseItem.Fields.Members_IsReadable, DirectoryBaseItem.Fields.Members_IsWritable, DirectoryBaseItem.Fields.Members_ModiefiedDateTime
            };

            var dir = await api
                .Directory
                .Get(path, pid, new[] { DirectoryMember.Directory, DirectoryMember.File }, fields)
                .ExecuteAsync();

            var items = dir.Members.Where(_ => _.IsReadable.Value && _.IsWritable.Value).Select(_ => new StorageProviderItem
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