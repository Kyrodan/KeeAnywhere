using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeeAnywhere.Configuration;
using KeePass.UI;

namespace KeeAnywhere.StorageProviders.AmazonS3
{
    public class AmazonS3StorageConfigurator : IStorageConfigurator
    {
        public async Task<AccountConfiguration> CreateAccount()
        {
            var dlg = new AmazonS3AccountForm();
            var result = UIUtil.ShowDialogAndDestroy(dlg);

            if (result != DialogResult.OK)
                return null;

            var account = new AccountConfiguration
            {
                Type = StorageType.AmazonS3,
                Id = dlg.AccessKey,
                Name = "S3 Account",
                Secret = dlg.SecretKey,
            };

            account.AdditionalSettings = new Dictionary<string, string>() {{"AWSRegion", dlg.AWSRegion.SystemName}};
            return account;
        }
    }
}
