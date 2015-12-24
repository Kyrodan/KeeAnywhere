using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.HubiC
{
    public class HubiCStorageProvider : IStorageProvider
    {
        private AccountConfiguration account;

        public HubiCStorageProvider(AccountConfiguration account)
        {
            this.account = account;
        }

        public Task<Stream> Load(string path)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Save(Stream stream, string path)
        {
            throw new NotImplementedException();
        }

        public Task<StorageProviderItem> GetRootItem()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent)
        {
            throw new NotImplementedException();
        }
    }
}
