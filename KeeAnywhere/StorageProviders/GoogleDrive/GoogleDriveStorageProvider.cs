using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.GoogleDrive
{
    public class GoogleDriveStorageProvider: IStorageProvider
    {
        public GoogleDriveStorageProvider(AccountConfiguration account)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string path)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> Load(string path)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Save(Stream stream, string path)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Move(string pathFrom, string pathTo)
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
