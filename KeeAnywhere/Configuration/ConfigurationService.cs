using System;
using System.Collections.Generic;
using System.Linq;
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
        private const string ConfigurationKey_Plugin = "KeeAnywhere.Plugin";
        private const string ConfigurationKey_Databases = "KeeAnywhere.Databases";
        private const string ConfigurationKey_Accounts = "KeeAnywhere.Accounts";

        private const string CredentialsStore_TargetPrefix = "KeeAnywhere";

        private readonly IPluginHost _pluginHost;

        public PluginConfiguration PluginConfiguration { get; private set; }

        public IList<AccountConfiguration> Accounts { get; private set; }

        public bool IsLoaded { get; private set; }

        public ConfigurationService(IPluginHost pluginHost)
        {
            if (pluginHost == null) throw new ArgumentNullException("pluginHost");

            _pluginHost = pluginHost;
        }

        public void Load()
        {
            if (IsLoaded) return;

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

        private void LoadAccountsFromDisk()
        {
            var configString = _pluginHost.CustomConfig.GetString(ConfigurationKey_Accounts);

            if (!string.IsNullOrEmpty(configString))
            {
                try
                {
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

        private void LoadAccountsFromWindowsCredentialManager()
        {
            var credentialSet = new CredentialSet();
            credentialSet.Load();
            var credentials = credentialSet.FindAll(c => c.Target.StartsWith(CredentialsStore_TargetPrefix));

            var accountsQuery = credentials.Select(c => new AccountConfiguration
            {
                Type = (StorageType)Enum.Parse(typeof(StorageType), c.Target.Substring(c.Target.IndexOf('.') + 1, (c.Target.IndexOf(':') - c.Target.IndexOf('.') - 1))),
                Name = c.Target.Substring(c.Target.IndexOf(':') + 1),
                Id = c.Username,
                Secret = c.Password,
            });

            this.Accounts = accountsQuery.ToList();
        }

        private void LoadPluginConfiguration()
        {
            var configString = _pluginHost.CustomConfig.GetString(ConfigurationKey_Plugin);

            if (!string.IsNullOrEmpty(configString))
            {
                try
                {
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

            if (account == null)
                throw new InvalidOperationException(string.Format("Account '{0}' for type '{1}' not found.", name, type));

            return account;
        }
    }
}