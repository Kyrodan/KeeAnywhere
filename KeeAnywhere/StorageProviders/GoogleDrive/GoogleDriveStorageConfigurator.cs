using System;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.GoogleDrive
{
    public class GoogleDriveStorageConfigurator : IStorageConfigurator
    {
        private readonly bool _isAccessRestricted;

        public GoogleDriveStorageConfigurator(bool isAccessRestricted)
        {
            this._isAccessRestricted = isAccessRestricted;
        }

        public async Task<AccountConfiguration> CreateAccount()
        {
            var flow = GoogleDriveHelper.CreateOidcFlow(_isAccessRestricted);
            return await flow.AuthorizeAsync();
        }
    }
}