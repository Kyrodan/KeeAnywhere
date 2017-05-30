using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using KeeAnywhere.Backup;
using KeeAnywhere.Configuration;
using KeeAnywhere.Offline;
using KeeAnywhere.WebRequest;
using KeePassLib.Serialization;

namespace KeeAnywhere.StorageProviders
{
    public class StorageService: IWebRequestCreate
    {
        private readonly ConfigurationService _configService;
        private readonly CacheManagerService _cacheManagerService;

        public StorageService(ConfigurationService configService, CacheManagerService cacheManagerService)
        {
            if (configService == null) throw new ArgumentNullException("configService");
            if (cacheManagerService == null) throw new ArgumentNullException("cacheManagerService");

            _configService = configService;
            _cacheManagerService = cacheManagerService;
        }

        public IStorageProvider GetProviderByUri(StorageUri uri)
        {
            var descriptor = StorageRegistry.Descriptors.FirstOrDefault(_ => _.Scheme == uri.Scheme);
            if (descriptor == null)
                throw new NotImplementedException(string.Format("A provider for scheme '{0}' is not implemented.",
                    uri.Scheme));

            var accountName = uri.GetAccountName();
            var account = _configService.FindAccount(descriptor.Type, accountName);
            if (account == null)
                throw new InvalidOperationException(string.Format("Account '{0}' for type '{1}' not found.", accountName, descriptor.Type));

            var provider = descriptor.ProviderFactory(account);

            return provider;
        }

        public IStorageProvider GetProviderByUri(string uriString)
        {
            var uri = new StorageUri(uriString);

            return GetProviderByUri(uri);
        }

        public IStorageProvider GetProviderByAccount(AccountConfiguration account)
        {
            if (account == null) throw new ArgumentNullException("account");

            var descriptor = StorageRegistry.Descriptors.FirstOrDefault(_ => _.Type == account.Type);
            if (descriptor == null)
                throw new NotImplementedException(string.Format("A provider for type '{0}' is not implemented.",
                    account.Type));

            var provider = descriptor.ProviderFactory(account);

            return provider;
        }

        public async Task<AccountConfiguration> CreateAccount(StorageType type)
        {
            var descriptor = StorageRegistry.Descriptors.FirstOrDefault(_ => _.Type == type);
            if (descriptor == null)
                throw new NotImplementedException(string.Format("A provider for type '{0}' is not implemented.", type));

            var configurator = descriptor.ConfiguratorFactory();
            var account = await configurator.CreateAccount();
   
            return account;
        }

        public System.Net.WebRequest Create(Uri uri)
        {
            var providerUri = new StorageUri(uri);
            var provider = this.GetProviderByUri(providerUri);


            // Pipeline: Backup => Cache => Basic Provider

            if (_configService.PluginConfiguration.IsOfflineCacheEnabled)
            {
                provider = _cacheManagerService.WrapInCacheProvider(provider, uri);
            }

            if (_configService.PluginConfiguration.IsBackupToLocalEnabled ||
                _configService.PluginConfiguration.IsBackupToRemoteEnabled)
            {
                provider = BackupProvider.WrapInBackupProvider(provider, providerUri, _configService);
            }

            var itemPath = providerUri.GetPath();

            return new KeeAnywhereWebRequest(provider, itemPath);
        }

        public void RegisterPrefixes()
        {
            foreach (var descriptor in StorageRegistry.Descriptors)
            {
                FileTransactionEx.Configure(descriptor.Scheme, false);
                System.Net.WebRequest.RegisterPrefix(descriptor.Scheme + ":", this);
            }
        }

        

       

    }
}