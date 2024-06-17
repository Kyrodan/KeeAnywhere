using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Auth.OAuth2;
using KeeAnywhere.Configuration;
using KeePass.UI;

namespace KeeAnywhere.StorageProviders.GoogleCloudPlatform
{
    public class GoogleCloudStorageConfigurator : IStorageConfigurator
    {
        public async Task<AccountConfiguration> CreateAccount()
        {
            var dlg = new GoogleCloudStorageAccountForm();
            var result = UIUtil.ShowDialogAndDestroy(dlg);

            if (result != DialogResult.OK)
                return null;

            var credentialsJson = File.ReadAllText(dlg.KeyFile);
            var credentials = GoogleCredential.FromJson(credentialsJson);
            var serviceCredentials = credentials.UnderlyingCredential as ServiceAccountCredential;

            var account = new AccountConfiguration
            {
                Type = StorageType.GoogleCloudStorage,
                Name = "Google Cloud Storage (" + dlg.ProjectId + ")",
                Id = serviceCredentials.Id,
                Secret = credentialsJson,
            };

            account.AdditionalSettings = new Dictionary<string, string>();
            account.AdditionalSettings.Add("ProjectId", dlg.ProjectId);

            return account;
        }
    }
}
