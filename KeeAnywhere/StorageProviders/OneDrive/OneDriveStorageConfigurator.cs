using System;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveStorageConfigurator : IStorageConfigurator
    {
        public async Task<AccountConfiguration> CreateAccount()
        {
            var flow = OneDriveHelper.CreateOidcFlow();
            return await flow.AuthorizeAsync();
        }
    }
}