using System;
using System.Linq;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using KeeAnywhere.WebRequest;
using KeePassLib.Serialization;

namespace KeeAnywhere.StorageProviders
{
    public class StorageService
    {
        private readonly ConfigurationService _configService;

        public StorageService(ConfigurationService configService)
        {
            if (configService == null) throw new ArgumentNullException("configService");
            _configService = configService;
        }

        public IStorageProvider GetProviderByUri(StorageUri uri)
        {
            var descriptor = StorageRegistry.Descriptors.FirstOrDefault(_ => _.Scheme == uri.Scheme);
            if (descriptor == null)
                throw new NotImplementedException(string.Format("A provider for scheme '{0}' is not implemented.",
                    uri.Scheme));

            var accountName = uri.GetAccountName();
            var account = _configService.FindAccount(descriptor.Type, accountName);
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

            //var drive = await _api.GetDrive();
            //var id = drive.Id;
            //var name = drive.Owner.User.DisplayName;
            //var refreshToken = _api.AccessToken.RefreshToken;

            //var account = configService.Accounts.SingleOrDefault(_ => _.Id == id);
            //if (account == null) // New Account
            //{
            //    account = new AccountConfiguration
            //    {
            //        Type = StorageType.OneDrive,
            //        Name = name,
            //        Id = id,
            //        RefreshToken = refreshToken,
            //    };

            //    configService.Accounts.Add(account);
            //}
            //else // Account exists: updating data
            //{
            //    MessageService.ShowInfo("This Account already exists.\r\nUpdating account data only.");

            //    account.Name = name;
            //    account.RefreshToken = refreshToken;
            //}

            return account;
        }


        public void Register()
        {
            foreach (var descriptor in StorageRegistry.Descriptors)
            {
                FileTransactionEx.Configure(descriptor.Scheme, false);
                System.Net.WebRequest.RegisterPrefix(descriptor.Scheme + ":", new KeeAnywhereWebRequestCreator(this));
            }
        }
    }
}