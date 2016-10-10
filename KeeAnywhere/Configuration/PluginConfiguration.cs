using System;
using System.IO;
using System.Runtime.Serialization;
using KeePass.App.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KeeAnywhere.Configuration
{
    [DataContract]
    public class PluginConfiguration
    {
        [DataMember]
        public bool IsOfflineCacheEnabled { get; set; }

        public string OfflineCacheFolder
        {
            get
            {
                return Path.Combine(AppConfigSerializer.LocalAppDataDirectory, "KeeAnywhereOfflineCache");
            }
        }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public AccountStorageLocation AccountStorageLocation { get; set; }

        [DataMember]
        public AccountIdentifier FilePickerLastUsedAccount { get; set; }

        [DataMember]
        public DateTime DonationDialogLastShown { get; set; }


        public PluginConfiguration()
        {
            this.IsOfflineCacheEnabled = true;
            this.AccountStorageLocation = AccountStorageLocation.LocalUserSecureStore;
            this.DonationDialogLastShown = DateTime.MinValue;
        }
    }
}