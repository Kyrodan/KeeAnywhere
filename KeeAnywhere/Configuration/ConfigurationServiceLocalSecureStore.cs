using System.Collections.Generic;
using System.IO;
using KeeAnywhere.Json;
using KeePass.App.Configuration;
using KeePassLib.Utility;
using Newtonsoft.Json;

namespace KeeAnywhere.Configuration
{
    partial class ConfigurationService
    {
        private const string ConfigurationFile_Accounts = "KeeAnywhere.Accounts.json";

        private void LoadAccountsFromLocalSecureStore()
        {
            this.MigrateDataToRoamingAppData();

            var path = ConfigurationInfo.SettingsDirectory;
            var filename = Path.Combine(path, ConfigurationFile_Accounts);

            if (!File.Exists(filename))
                return;

            var configString = File.ReadAllText(filename);

            if (string.IsNullOrEmpty(configString)) return;

            try
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new ProtectedDataStringPropertyResolver()
                };

                this.Accounts = JsonConvert.DeserializeObject<IList<AccountConfiguration>>(configString, settings);
            }
            catch (JsonSerializationException)
            {
                MessageService.ShowWarning(
                    "KeeAnywhere Plugin:",
                    "Unable to parse the plugin's account configuration for the KeeAnywhere plugin from local secure store.");
            }
        }

        private void SaveAccountsToLocalSecureStore()
        {
            var path = ConfigurationInfo.SettingsDirectory;
            var filename = Path.Combine(path, "KeeAnywhere.Accounts.json");

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new ProtectedDataStringPropertyResolver()
            };

            var configString = JsonConvert.SerializeObject(this.Accounts, settings);

            File.WriteAllText(filename, configString);
        }

        // v1.5.0: #113 Adjust account storage location
        public void MigrateDataToRoamingAppData()
        {
            if (ConfigurationInfo.IsPortable) { return; }

            var filenameOld = Path.Combine(AppConfigSerializer.LocalAppDataDirectory, ConfigurationFile_Accounts);
            var filenameNew = Path.Combine(ConfigurationInfo.SettingsDirectory, ConfigurationFile_Accounts);

            if (!File.Exists(filenameOld)) { return; }

            try
            {
                File.Move(filenameOld, filenameNew);
            }
            catch { }
        }

    }
}
