using System;

namespace KeeAnywhere.StorageProviders
{
    public class StorageProviderDescriptor
    {
        public StorageProviderDescriptor(StorageProviderType type, string scheme, Func<string, IStorageProvider> factory)
        {
            Type = type;
            Scheme = scheme;
            Factory = factory;
        }
        public StorageProviderType Type { get; private set; }
        public string Scheme { get; private set; }
        public Func<string, IStorageProvider> Factory { get; private set; }
    }
}