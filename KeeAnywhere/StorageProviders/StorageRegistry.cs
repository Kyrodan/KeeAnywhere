using System.Collections.Generic;
using KeeAnywhere.StorageProviders.OneDrive;

namespace KeeAnywhere.StorageProviders
{
    public static class StorageRegistry
    {
        public static IEnumerable<StorageDescriptor> Descriptors = new[]
        {
            new StorageDescriptor(StorageType.OneDrive, "onedrive", account => new OneDriveStorageProvider(account), () => new OneDriveStorageConfigurator())
        };
    }
}