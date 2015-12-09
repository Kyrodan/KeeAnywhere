using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KeeAnywhere.StorageProviders
{
    public interface IStorageProvider
    {
        // File operations
        Task<bool> Delete(string path);
        Task<Stream> Load(string path);
        Task<bool> Save(MemoryStream stream, string path);
        Task<bool> Move(string pathFrom, string pathTo);

        // Query operations
        Task<StorageProviderItem> GetRootItem();
        Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent);
    }
}