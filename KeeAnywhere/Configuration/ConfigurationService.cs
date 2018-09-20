using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using KeeAnywhere.StorageProviders;
using KeePass.Plugins;
using KeePassLib.Utility;
using Newtonsoft.Json;

namespace KeeAnywhere.Configuration
{
    public partial class ConfigurationService
    {
        private const string ConfigurationKey_LastUsedPluginVersion = "KeeAnywhere.LastUsedPluginVersion";
        private const string ConfigurationKey_Plugin = "KeeAnywhere.Plugin";

        private readonly IPluginHost _pluginHost;

        public PluginConfiguration PluginConfiguration { get; private set; }

        public IList<AccountConfiguration> Accounts { get; private set; }

        public Version LastUsedPluginVersion { get; private set; }

        public Version CurrentPluginVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

        public bool IsLoaded { get; private set; }

        public bool IsUpgraded { get; private set; }

        public ConfigurationService(IPluginHost pluginHost)
        {
            if (pluginHost == null) throw new ArgumentNullException("pluginHost");

            _pluginHost = pluginHost;
        }

        public void Load()
        {
            if (IsLoaded) return;

            this.LastUsedPluginVersion = LoadLastUsedPluginVersion();
            this.IsUpgraded = this.LastUsedPluginVersion < this.CurrentPluginVersion;

            LoadPluginConfiguration();

            switch (PluginConfiguration.AccountStorageLocation)
            {
                case AccountStorageLocation.KeePassConfig:
                    LoadAccountsFromKeePassConfig();
                    break;

                case AccountStorageLocation.WindowsCredentialManager:
                    LoadAccountsFromWindowsCredentialManager();
                    this.PluginConfiguration.AccountStorageLocation = AccountStorageLocation.LocalUserSecureStore;

                    MessageService.ShowWarning(
                        "KeeAnywhere Plugin:",
                        "Account Storage Location 'Windows Credential Manager' is deprecated since plugin version 1.3.0.",
                        "Changed configuration to use new default 'Local User Secure Store'."
                        );

                    break;

                case AccountStorageLocation.LocalUserSecureStore:
                    LoadAccountsFromLocalSecureStore();
                    break;

                default:
                    throw new NotImplementedException(string.Format("AccountStorageLocation {0} not implemented.", PluginConfiguration.AccountStorageLocation));
            }

            if (this.Accounts == null)
                this.Accounts = new List<AccountConfiguration>();

            IsLoaded = true;
        }

        public void Save()
        {
            if (!IsLoaded) return;

            SavePluginConfiguration();

            switch (PluginConfiguration.AccountStorageLocation)
            {
                case AccountStorageLocation.KeePassConfig:
                    SaveAccountsToKeePassConfig();
                    break;

                case AccountStorageLocation.WindowsCredentialManager:
                    SaveAccountsToWindowsCredentialManager();
                    break;

                case AccountStorageLocation.LocalUserSecureStore:
                    SaveAccountsToLocalSecureStore();
                    break;

                default:
                    throw new NotImplementedException(string.Format("AccountStorageLocation {0} not implemented.", PluginConfiguration.AccountStorageLocation));
            }

            SaveLastUsedPluginVersion();
        }


        private Version LoadLastUsedPluginVersion()
        {
            var lastUsedPluginVersionString = _pluginHost.CustomConfig.GetString(ConfigurationKey_LastUsedPluginVersion);
            if (string.IsNullOrEmpty(lastUsedPluginVersionString)) return new Version(0, 0, 0, 0);

            try
            {
                return new Version(lastUsedPluginVersionString);
            }
            catch (Exception)
            {
                return new Version(0, 0, 0, 0);
            }
        }

        private void SaveLastUsedPluginVersion()
        {
            this.LastUsedPluginVersion = this.CurrentPluginVersion;
            _pluginHost.CustomConfig.SetString(ConfigurationKey_LastUsedPluginVersion, this.LastUsedPluginVersion.ToString());
        }

        private void LoadPluginConfiguration()
        {
            var configString = _pluginHost.CustomConfig.GetString(ConfigurationKey_Plugin);

            if (!string.IsNullOrEmpty(configString))
            {
                try
                {
                    if (this.LastUsedPluginVersion < new Version(1, 3))
                        configString = configString.MigratePluginConfigurationTo130();

                    this.PluginConfiguration = JsonConvert.DeserializeObject<PluginConfiguration>(configString);
                }
                catch (JsonSerializationException)
                {
                    MessageService.ShowWarning(
                        "KeeAnywhere Plugin:",
                        "Unable to parse the plugin configuration for the KeeAnywhere plugin.");
                }
            }

            if (PluginConfiguration == null)
            {
                this.PluginConfiguration = new PluginConfiguration();
            }

            if (string.IsNullOrEmpty(this.PluginConfiguration.BackupToLocalFolder))
            {
                this.PluginConfiguration.BackupToLocalFolder = Path.Combine(ConfigurationInfo.SettingsDirectory,
                    "KeeAnywhereBackups");
            }

            if (this.PluginConfiguration.BackupCopies < 1)
                this.PluginConfiguration.BackupCopies = 10;
        }

        private void SavePluginConfiguration()
        {
            var configString = JsonConvert.SerializeObject(this.PluginConfiguration);
            _pluginHost.CustomConfig.SetString(ConfigurationKey_Plugin, configString);
        }

        public AccountConfiguration FindAccount(StorageType type, string name)
        {
            var account = this.Accounts.FirstOrDefault(_ => _.Type == type && _.Name == name);

            //if (account == null)
            //    throw new InvalidOperationException(string.Format("Account '{0}' for type '{1}' not found.", name, type));

            return account;
        }

    }
}