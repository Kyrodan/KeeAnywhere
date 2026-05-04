using System;
using System.Drawing;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders
{
    public class StorageDescriptor
    {
        public StorageDescriptor(StorageType type, string friendlyName, string scheme, Func<AccountConfiguration, IStorageProvider> providerFactory, Func<IStorageConfigurator> configuratorFactory, Image smallImage, Action<string> invalidateCacheAction = null)
        {
            Type = type;
            FriendlyName = friendlyName;
            Scheme = scheme;
            ProviderFactory = providerFactory;
            ConfiguratorFactory = configuratorFactory;
            SmallImage = smallImage;
            InvalidateCacheAction = invalidateCacheAction;
        }

        public StorageType Type { get; private set; }
        public string FriendlyName { get; set; }
        public string Scheme { get; private set; }
        public Func<AccountConfiguration, IStorageProvider> ProviderFactory { get; private set; }
        public Func<IStorageConfigurator> ConfiguratorFactory { get; private set; }
        public Image SmallImage { get; private set; }
        public Action<string> InvalidateCacheAction { get; private set; }
    }
}