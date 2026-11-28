using System;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveStorageConfigurator : IStorageConfigurator
    {
        private readonly bool _isAccessRestricted;

        public OneDriveStorageConfigurator(bool isAccessRestricted)
        {
            _isAccessRestricted = isAccessRestricted;
        }

        public async Task<AccountConfiguration> CreateAccount()
        {
            var flow = OneDriveHelper.CreateOidcFlow(_isAccessRestricted);
            return await flow.AuthorizeAsync();
        }
    }
}