using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.HubiC
{
    public class HubiCStorageProvider : IStorageProvider
    {
        private readonly AccountConfiguration _account;
        private SwiftClient _client;
        private string _defaultContainer;

        public HubiCStorageProvider(AccountConfiguration account)
        {
            if (account == null) throw new ArgumentNullException("account");
            this._account = account;
        }

        public async Task<Stream> Load(string path)
        {
            if (path == null) throw new ArgumentNullException("path");

            var container = await GetDefaultContainer();

            var client = await GetClient();
            var normalizedPath = path.StartsWith("/") ? path.Remove(0, 1) : path;

            var stream = await client.DownloadObject(container, normalizedPath);

            return stream;
        }

        public async Task<bool> Save(Stream stream, string path)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (path == null) throw new ArgumentNullException("path");

            var container = await GetDefaultContainer();

            var client = await GetClient();
            var normalizedPath = path.StartsWith("/") ? path.Remove(0, 1) : path;
            var folderName = CloudPath.GetDirectoryName(normalizedPath);

            var folder = await client.GetObjects(container, folderName);
            if (folder == null || !folder.Any())
                throw new InvalidOperationException(string.Format("Folder does not exist: {0}", folderName));

            var isOk = await client.UploadObject(container, normalizedPath, stream);

            return isOk;
        }

        public async Task<StorageProviderItem> GetRootItem()
        {
            var container = await GetDefaultContainer();

            return new StorageProviderItem {
                Id = "/",
                Name = "/",
                Type = StorageProviderItemType.Folder
            };
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent)
        {
            var client = await GetClient();
            var container = await GetDefaultContainer();
            var objects = await client.GetObjects(container, parent.Id);

            var ret = objects.Select(_ => new StorageProviderItem
            {
                Type =
                    _.ContentType == "application/directory"
                        ? StorageProviderItemType.Folder
                        : StorageProviderItemType.File,
                Name = NormalizeName(_.Name),
                Id = _.Name,
                ParentReferenceId = parent.Id,
                LastModifiedDateTime = _.LastModified
            });

            return ret.ToArray();
        }

        public bool IsFilenameValid(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;

            char[] invalidChars = { '/', '\\' };
            return filename.All(c => c >= 32 && !invalidChars.Contains(c));
        }

        protected string NormalizeName(string path)
        {
            var ret = path.EndsWith("/") ? path.Remove(path.Length - 1, 1) : path;

            var posOfLastSlash = ret.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase);

            return posOfLastSlash >= 0 ? ret.Substring(posOfLastSlash + 1) : ret;
        }

        protected async Task<SwiftClient> GetClient()
        {
            if (_client != null) return _client;

            var token = await HubiCHelper.GetAccessTokenAsync(_account.Secret);
            var credentials = await HubiCHelper.GetCredentialsAsync(token.AccessToken);
            _client = new SwiftClient(credentials);

            return _client;
        }

        protected async Task<string> GetDefaultContainer()
        {
            if (_defaultContainer != null) return _defaultContainer;

            var client = await GetClient();
            var containers = await client.GetContainers();
            var container =
                containers.Single(_ => _.Name.Equals("default", StringComparison.InvariantCultureIgnoreCase));

            _defaultContainer = container.Name;

            return _defaultContainer;
        }
    }
}