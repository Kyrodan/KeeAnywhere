using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using CredentialManagement;
using KeeAnywhere.StorageProviders;
using KeePass.Plugins;
using KeePassLib.Utility;
using Newtonsoft.Json;

namespace KeeAnywhere.Configuration
{
    public class ConfigurationService
    {
        private const string ConfigurationKey_LastUsedPluginVersion = "KeeAnywhere.LastUsedPluginVersion";
        private const string ConfigurationKey_Plugin = "KeeAnywhere.Plugin";
        private const string ConfigurationKey_Accounts = "KeeAnywhere.Accounts";

        private const string CredentialsStore_TargetPrefix = "KeeAnywhere";

        private readonly IPluginHost _pluginHost;

        public PluginConfiguration PluginConfiguration { get; private set; }

        public IList<AccountConfiguration> Accounts { get; private set; }

        public Version LastUsedPluginVersion { get; private set; }

        public Version CurrentPluginVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

        public bool IsLoaded { get; private set; }

        public ConfigurationService(IPluginHost pluginHost)
        {
            if (pluginHost == null) throw new ArgumentNullException("pluginHost");

            _pluginHost = pluginHost;
        }

        public void Load()
        {
            if (IsLoaded) return;

            this.LastUsedPluginVersion = LoadLastUsedPluginVersion();

            LoadPluginConfiguration();

            switch (PluginConfiguration.AccountStorageLocation)
            {
                case AccountStorageLocation.KeePassConfig:
                    LoadAccountsFromDisk();
                    break;

                case AccountStorageLocation.WindowsCredentialManager:
                    LoadAccountsFromWindowsCredentialManager();
                    break;

                default:
                    throw new NotImplementedException(string.Format("AccountStorageLocation {0} not implemented.", PluginConfiguration.AccountStorageLocation));
            }


            IsLoaded = true;
        }

        private Version LoadLastUsedPluginVersion()
        {
            var lastUsedPluginVersionString = _pluginHost.CustomConfig.GetString(ConfigurationKey_LastUsedPluginVersion);
            if (string.IsNullOrEmpty(lastUsedPluginVersionString)) return null;

            try
            {
                return new Version(lastUsedPluginVersionString);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void LoadAccountsFromDisk()
        {
            var configString = _pluginHost.CustomConfig.GetString(ConfigurationKey_Accounts);

            if (!string.IsNullOrEmpty(configString))
            {
                try
                {
                    if (this.LastUsedPluginVersion == null || this.LastUsedPluginVersion < new Version(1, 3))
                        configString = MigrateAccountTo130(configString);

                    this.Accounts = JsonConvert.DeserializeObject<IList<AccountConfiguration>>(configString);
                }
                catch (JsonSerializationException)
                {
                    MessageService.ShowWarning(
                        "Unable to parse the plugin configuration for the KeeAnywhere plugin. If this happens again, please let me know. Sorry for the inconvinience. Koen Zomers <mail@koenzomers.nl>",
                        "KeeAnywhere Plugin");
                }
            }

            if (Accounts == null)
            {
                this.Accounts = new List<AccountConfiguration>();
            }
        }

        private string MigratePluginConfigurationTo130(string configString)
        {
            var s = configString;
            s = s.Replace("\"AccountStorageLocation\":0", "\"AccountStorageLocation\":\"KeePassConfig\"");
            s = s.Replace("\"AccountStorageLocation\":1", "\"AccountStorageLocation\":\"WindowsCredentialManager\"");

            s = MigrateAccountTo130(s); // For Last Used Account

            return s;
        }

        private string MigrateAccountTo130(string configString)
        {
            var s = configString;
            s = s.Replace("\"Type\":0", "\"Type\":\"Dropbox\"");
            s = s.Replace("\"Type\":1", "\"Type\":\"DropboxRestricted\"");
            s = s.Replace("\"Type\":2", "\"Type\":\"GoogleDrive\"");
            s = s.Replace("\"Type\":3", "\"Type\":\"HubiC\"");
            s = s.Replace("\"Type\":4", "\"Type\":\"OneDrive\"");

            return s;
        }

        private void LoadAccountsFromWindowsCredentialManager()
        {
            var credentialSet = new CredentialSet();
            credentialSet.Load();
            var credentials = credentialSet.FindAll(c => c.Target.StartsWith(CredentialsStore_TargetPrefix));

            StorageType type;
            var filterQuery = credentials.Where(c => Enum.TryParse(GetCredentialTypeAsString(c), out type));
            var accountsQuery = filterQuery.Select(c => new AccountConfiguration
            {
                Type = (StorageType)Enum.Parse(typeof(StorageType), GetCredentialTypeAsString(c)),
                Name = c.Target.Substring(c.Target.IndexOf(':') + 1),
                Id = c.Username,
                Secret = c.Password,
            });

            this.Accounts = accountsQuery.ToList();
        }

        private static string GetCredentialTypeAsString(Credential c)
        {
            return c.Target.Substring(c.Target.IndexOf('.') + 1, (c.Target.IndexOf(':') - c.Target.IndexOf('.') - 1));
        }

        private void LoadPluginConfiguration()
        {
            var configString = _pluginHost.CustomConfig.GetString(ConfigurationKey_Plugin);

            if (!string.IsNullOrEmpty(configString))
            {
                try
                {
                    if (this.LastUsedPluginVersion == null || this.LastUsedPluginVersion < new Version(1, 3))
                        configString = MigratePluginConfigurationTo130(configString);

                    this.PluginConfiguration = JsonConvert.DeserializeObject<PluginConfiguration>(configString);
                }
                catch (JsonSerializationException)
                {
                    MessageService.ShowWarning(
                        "Unable to parse the plugin configuration for the KeeAnywhere plugin. If this happens again, please let me know. Sorry for the inconvinience. Koen Zomers <mail@koenzomers.nl>",
                        "KeeAnywhere Plugin");
                }
            }

            if (PluginConfiguration == null)
            {
                this.PluginConfiguration = new PluginConfiguration();
            }
        }

        public void Save()
        {
            if (!IsLoaded) return;

            SavePluginConfiguration();

            switch (PluginConfiguration.AccountStorageLocation)
            {
                case AccountStorageLocation.KeePassConfig:
                    SaveAccountsToDisk();
                    break;

                case AccountStorageLocation.WindowsCredentialManager:
                    SaveAccountsToWindowsCredentialManager();
                    break;

                default:
                    throw new NotImplementedException(string.Format("RefreshTokeStorage {0} not implemented.", PluginConfiguration.AccountStorageLocation));
            }

            SaveLastUsedPluginVersion();
        }

        private void SaveLastUsedPluginVersion()
        {
            _pluginHost.CustomConfig.SetString(ConfigurationKey_LastUsedPluginVersion, this.CurrentPluginVersion.ToString());
        }

        private void SaveAccountsToWindowsCredentialManager()
        {
            var credentialsQuery =
                this.Accounts.Select(a => new Credential
                {
                    Target = string.Format("{0}.{1}:{2}", CredentialsStore_TargetPrefix, a.Type, a.Name),
                    Username =  a.Id,
                    Password = a.Secret,
                    PersistanceType = PersistanceType.LocalComputer,
                    Type = CredentialType.Generic
                });

            var credentials = credentialsQuery.ToArray();


            // Save changed credentials to Credential Store
            foreach (var credential in credentials)
            {
                credential.Save();
            }


            // Remove deleted credentials from Credential Store
            var credentialSet = new CredentialSet();
            credentialSet.Load();
            var credentialsToDelete = credentialSet.FindAll(toDelete => toDelete.Target.StartsWith(CredentialsStore_TargetPrefix) && credentials.All(_ => toDelete.Target != _.Target));
            foreach (var credential in credentialsToDelete)
            {
                credential.Delete();
            }

        }

        private void SaveAccountsToDisk()
        {
            var configString = JsonConvert.SerializeObject(this.Accounts);
            _pluginHost.CustomConfig.SetString(ConfigurationKey_Accounts, configString);
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