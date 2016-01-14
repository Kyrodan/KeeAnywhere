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

        public HubiCStorageProvider(AccountConfiguration account)
        {
            if (account == null) throw new ArgumentNullException("account");
            this._account = account;
        }

        public Task<Stream> Load(string path)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Save(Stream stream, string path)
        {
            throw new NotImplementedException();
        }

        public async Task<StorageProviderItem> GetRootItem()
        {
            var client = await GetClient();
            var containers = await client.GetContainers();
            var root = containers.First();

            return new StorageProviderItem {Id = root.Name, Name = root.Name, Type = StorageProviderItemType.Folder};
        }

        public async Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent)
        {
            var client = await GetClient();
            var id = parent.Id;
            var posOfFirstSlash = id.IndexOf("/", StringComparison.InvariantCultureIgnoreCase);

            var container = posOfFirstSlash >= 0 ? id.Substring(0, posOfFirstSlash) : id;
            var path = posOfFirstSlash >= 0 ? id.Substring(posOfFirstSlash + 1) : null;


            var objects = await client.GetObjects(container, path);

            var ret = objects.Select(_ => new StorageProviderItem
            {
                Type =
                    _.ContentType == "application/directory"
                        ? StorageProviderItemType.Folder
                        : StorageProviderItemType.File,
                Name = NormalizeName(_.Name),
                Id = container + "/" + _.Name,
                ParentReferenceId = id,
                LastModifiedDateTime = _.LastModified
            });

            return ret.ToArray();
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
    }
}