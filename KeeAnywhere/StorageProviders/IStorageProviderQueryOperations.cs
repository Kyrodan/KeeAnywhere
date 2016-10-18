using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeeAnywhere.StorageProviders
{
    public interface IStorageProviderQueryOperations
    {
        // Query operations
        Task<StorageProviderItem> GetRootItem();
        Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent);
        Task<IEnumerable<StorageProviderItem>> GetChildrenByParentPath(string path);
    }
}