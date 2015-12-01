using System;
using System.Linq;
using KeeAnywhere.Configuration;
using KeeAnywhere.WebRequest;
using KeePassLib.Serialization;

namespace KeeAnywhere.StorageProviders
{
    public class StorageProviderService
    {

        private readonly ConfigurationService _configService;

        public StorageProviderService(ConfigurationService configService)
        {
            if (configService == null) throw new ArgumentNullException("configService");
            _configService = configService;
        }

        public IStorageProvider GetByUri(StorageProviderUri uri)
        {
            var descriptor = StorageProviderRegistry.Descriptors.FirstOrDefault(_ => _.Scheme == uri.Scheme);
            if (descriptor == null)
                throw new NotImplementedException(string.Format("A provider for scheme '{0}' is not implemented.",
                    uri.Scheme));

            var accountName = uri.GetAccountName();
            var account = _configService.FindAccount(descriptor.Type, accountName);
            var provider = descriptor.Factory(account.RefreshToken);

            return provider;
        }

        public IStorageProvider GetByUri(string uriString)
        {
            var uri = new StorageProviderUri(uriString);

            return GetByUri(uri);
        }

        public void Register()
        {
            foreach (var descriptor in StorageProviderRegistry.Descriptors)
            {
                FileTransactionEx.Configure(descriptor.Scheme, false);
                System.Net.WebRequest.RegisterPrefix(descriptor.Scheme + ":", new KeeAnywhereWebRequestCreator(this));
            }
        }
    }
}