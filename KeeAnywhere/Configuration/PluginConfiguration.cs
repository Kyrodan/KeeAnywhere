using System.Runtime.Serialization;

namespace KeeAnywhere.Configuration
{
    [DataContract]
    public class PluginConfiguration
    {
        [DataMember]
        public bool IsOfflineCacheEnabled { get; set; }

        [DataMember]
        public AccountStorageLocation AccountStorageLocation { get; set; }

        [DataMember]
        public AccountIdentifier FilePickerLastUsedAccount { get; set; }

        public PluginConfiguration()
        {
            this.IsOfflineCacheEnabled = true;
            this.AccountStorageLocation = AccountStorageLocation.WindowsCredentialManager;
        }
    }
}