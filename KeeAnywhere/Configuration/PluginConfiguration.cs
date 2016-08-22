using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KeeAnywhere.Configuration
{
    [DataContract]
    public class PluginConfiguration
    {
        [DataMember]
        public bool IsOfflineCacheEnabled { get; set; }

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
            this.AccountStorageLocation = AccountStorageLocation.WindowsCredentialManager;
            this.DonationDialogLastShown = DateTime.MinValue;
        }
    }
}