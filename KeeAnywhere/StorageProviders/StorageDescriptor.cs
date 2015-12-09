using System;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders
{
    public class StorageDescriptor
    {
        public StorageDescriptor(StorageType type, string scheme, Func<AccountConfiguration, IStorageProvider> providerFactory, Func<IStorageConfigurator> configuratorFactory)
        {
            Type = type;
            Scheme = scheme;
            ProviderFactory = providerFactory;
            ConfiguratorFactory = configuratorFactory;
        }

        public StorageType Type { get; private set; }
        public string Scheme { get; private set; }
        public Func<AccountConfiguration, IStorageProvider> ProviderFactory { get; private set; }
        public Func<IStorageConfigurator> ConfiguratorFactory { get; private set; }
    }
}