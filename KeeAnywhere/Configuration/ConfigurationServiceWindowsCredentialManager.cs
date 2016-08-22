using System;
using System.Collections.Generic;
using System.Linq;
using CredentialManagement;
using KeeAnywhere.StorageProviders;
using Newtonsoft.Json;

namespace KeeAnywhere.Configuration
{
    partial class ConfigurationService
    {
        private const string CredentialsStore_TargetPrefix = "KeeAnywhere";

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
                AdditionalSettings = !string.IsNullOrEmpty(c.Description) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(c.Description) : null

            });

            this.Accounts = accountsQuery.ToList();
        }

        private void SaveAccountsToWindowsCredentialManager()
        {
            var credentialsQuery =
                this.Accounts.Select(a => new Credential
                {
                    Target = string.Format("{0}.{1}:{2}", CredentialsStore_TargetPrefix, a.Type, a.Name),
                    Username = a.Id,
                    Password = a.Secret,
                    PersistanceType = PersistanceType.LocalComputer,
                    Type = CredentialType.Generic,
                    Description = a.AdditionalSettings != null ? JsonConvert.SerializeObject(a.AdditionalSettings) : null
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

        private static string GetCredentialTypeAsString(Credential c)
        {
            return c.Target.Substring(c.Target.IndexOf('.') + 1, (c.Target.IndexOf(':') - c.Target.IndexOf('.') - 1));
        }
    }
}
