using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KeeAnywhere.StorageProviders
{
    public interface IStorageProvider
    {
        // File operations
        Task<Stream> Load(string path);
        Task<bool> Save(Stream stream, string path);

        // Query operations
        Task<StorageProviderItem> GetRootItem();
        Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent);

        // Validation operations
        bool IsFilenameValid(string filename);
    }
}