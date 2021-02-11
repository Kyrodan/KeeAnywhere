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
            var flow = new OidcFlow(StorageType.GoogleDrive, GoogleDriveHelper.GoogleDriveClientId, GoogleDriveHelper.GoogleDriveClientSecret, GoogleDriveHelper.Scopes);
            return await flow.AuthorizeOidAsync("https://accounts.google.com");
        }
    }
}