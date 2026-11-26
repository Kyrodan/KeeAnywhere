using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeeAnywhere.Configuration;
using KeePass.UI;

namespace KeeAnywhere.StorageProviders.Azure
{
    public class AzureStorageConfigurator : IStorageConfigurator
    {
        private readonly StorageType _storageType;

        public AzureStorageConfigurator(StorageType storageType)
        {
            if (!(storageType == StorageType.AzureBlob
                  || storageType == StorageType.AzureFile))
                throw new ArgumentException("Only Azure storage types are allowed for configuration.", "storageType");

            _storageType = storageType;
        }

        public async Task<AccountConfiguration> CreateAccount()
        {
            var dlg = new AzureAccountForm(_storageType);
            var result = await Task.Run(() => UIUtil.ShowDialogAndDestroy(dlg));

            if (result != DialogResult.OK)
                return null;

            var account = new AccountConfiguration
            {
                Type = _storageType,
                // Unique per storage account and container/share, so multiple Azure accounts can be added (#351)
                Id = dlg.AccountName + "/" + dlg.ItemName,
                Name = dlg.AccountName,
                Secret = dlg.AccessToken,
                AdditionalSettings = new Dictionary<string, string>() { { "AzureItemName", dlg.ItemName } }
            };

            return account;
        }
    }
}