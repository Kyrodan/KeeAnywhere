using System.Collections.Generic;
using KeeAnywhere.StorageProviders.OneDrive;

namespace KeeAnywhere.StorageProviders
{
    public static class StorageProviderRegistry
    {
        public static IEnumerable<StorageProviderDescriptor> Descriptors = new[]
        {
            new StorageProviderDescriptor(StorageProviderType.OneDrive, "onedrive", token => new OneDriveStorageProvider(token))
        };
    }
}