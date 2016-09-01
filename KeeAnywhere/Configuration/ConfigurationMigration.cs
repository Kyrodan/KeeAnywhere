using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeAnywhere.Configuration
{
    public static class ConfigurationMigration
    {
        public static string MigratePluginConfigurationTo130(this string configString)
        {
            var s = configString;
            s = s.Replace("\"AccountStorageLocation\":0", "\"AccountStorageLocation\":\"KeePassConfig\"");
            s = s.Replace("\"AccountStorageLocation\":1", "\"AccountStorageLocation\":\"WindowsCredentialManager\"");

            s = MigrateAccountTo130(s); // For Last Used Account

            return s;
        }

        public static string MigrateAccountTo130(this string configString)
        {
            var s = configString;
            s = s.Replace("\"Type\":0", "\"Type\":\"Dropbox\"");
            s = s.Replace("\"Type\":1", "\"Type\":\"DropboxRestricted\"");
            s = s.Replace("\"Type\":2", "\"Type\":\"GoogleDrive\"");
            s = s.Replace("\"Type\":3", "\"Type\":\"HubiC\"");
            s = s.Replace("\"Type\":4", "\"Type\":\"OneDrive\"");

            return s;
        }

    }
}
