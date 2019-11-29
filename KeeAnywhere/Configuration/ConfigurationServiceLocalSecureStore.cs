using System.Collections.Generic;
using System.Diagnostics;
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
                var isEncryptionError = false;

                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new ProtectedDataStringPropertyResolver(),
                    Error = (s, args) =>
                    {
                        if (args.ErrorContext.Error is JsonDecryptException)
                        {
                            isEncryptionError = true;
                            args.ErrorContext.Handled = true;
                        }
                    }
                };

                this.Accounts = JsonConvert.DeserializeObject<IList<AccountConfiguration>>(configString, settings);

                if (isEncryptionError)
                {
                    var openTroubleshooting = MessageService.AskYesNo(
                        "Unable to parse the plugin's account configuration for the KeeAnywhere plugin from local secure store:\n" +
                        "Decryption of screts failed.\n\nWould you like to check Troubleshooting Documentation for that?",
                        "KeeAnywhere Plugin:"
                    );
                    if (openTroubleshooting)
                    {
                        Process.Start("https://keeanywhere.de/use/troubleshooting#could-not-read-account-configuration");
                    }
                }
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
                NullValueHandling = NullValueHandling.Ignore,
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
