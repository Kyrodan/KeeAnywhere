using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KeeAnywhere.Configuration
{
    [DataContract]
    public class PluginConfiguration
    {
        private string _backupToLocalFolder;

        [DataMember]
        public bool IsOfflineCacheEnabled { get; set; }

        public string OfflineCacheFolder
        {
            get
            {
                return Path.Combine(ConfigurationInfo.SettingsDirectory, "KeeAnywhereOfflineCache");
            }
        }

        [DataMember]
        public bool IsBackupToLocalEnabled { get; set; }

        [DataMember]
        public bool IsBackupToRemoteEnabled { get; set; }

        [DataMember]
        public string BackupToLocalFolder
        {
            get { return _backupToLocalFolder; }
            set
            {
                _backupToLocalFolder = value;
                if (this.IsBackupToLocalEnabled && !string.IsNullOrEmpty(_backupToLocalFolder) && !Directory.Exists(_backupToLocalFolder))
                {
                    try
                    {
                        Directory.CreateDirectory(_backupToLocalFolder);
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        [DataMember]
        [DefaultValue(10)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int BackupCopies { get; set; }

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
            this.DonationDialogLastShown = DateTime.Today;
        }
    }
}