using System;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;

namespace KeeAnywhere.StorageProviders.GoogleDrive
{
    public class GoogleDriveStorageConfigurator : IStorageConfigurator
    {
        public async Task<AccountConfiguration> CreateAccount()
        {
            var flow = new OidcFlow(StorageType.GoogleDrive, "https://accounts.google.com", GoogleDriveHelper.Scopes, GoogleDriveHelper.GoogleDriveClientId, GoogleDriveHelper.GoogleDriveClientSecret);
            return await flow.AuthorizeAsync();
        }
    }
}