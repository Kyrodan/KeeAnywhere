using System;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.GoogleDrive
{
    public class GoogleDriveStorageConfigurator : IStorageConfigurator
    {
        public async Task<AccountConfiguration> CreateAccount()
        {
            var flow = GoogleDriveHelper.CreateOidcFlow();
            return await flow.AuthorizeAsync();
        }
    }
}