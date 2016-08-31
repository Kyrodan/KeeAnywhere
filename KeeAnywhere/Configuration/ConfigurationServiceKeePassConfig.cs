using System;
using System.Collections.Generic;
using KeePassLib.Utility;
using Newtonsoft.Json;

namespace KeeAnywhere.Configuration
{
    partial class ConfigurationService
    {
        private const string ConfigurationKey_Accounts = "KeeAnywhere.Accounts";

        private void LoadAccountsFromKeePassConfig()
        {
            var configString = _pluginHost.CustomConfig.GetString(ConfigurationKey_Accounts);

            if (string.IsNullOrEmpty(configString)) return;

            try
            {
                if (this.LastUsedPluginVersion < new Version(1, 3))
                    configString = configString.MigrateAccountTo130();

                this.Accounts = JsonConvert.DeserializeObject<IList<AccountConfiguration>>(configString);
            }
            catch (JsonSerializationException)
            {
                MessageService.ShowWarning(
                    "KeeAnywhere Plugin:",
                    "Unable to parse the plugin's account configuration for the KeeAnywhere plugin from KeePass config.");
            }
        }

        private void SaveAccountsToKeePassConfig()
        {
            var configString = JsonConvert.SerializeObject(this.Accounts);
            _pluginHost.CustomConfig.SetString(ConfigurationKey_Accounts, configString);
        }
    }
}
