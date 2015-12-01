using System.Runtime.Serialization;

namespace KeeAnywhere.Configuration
{
    [DataContract]
    public class PluginConfiguration
    {
        [DataMember]
        public bool IsOfflineCacheEnabled { get; set; }

        [DataMember]
        public RefreshTokenStorage RefreshTokenStorage { get; set; }

        public PluginConfiguration()
        {
            this.IsOfflineCacheEnabled = true;
            this.RefreshTokenStorage = RefreshTokenStorage.WindowsCredentialManager;
        }
    }
}