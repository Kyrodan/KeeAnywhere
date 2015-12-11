using System.Runtime.Serialization;
using KeeAnywhere.StorageProviders;

namespace KeeAnywhere.Configuration
{
    [DataContract]
    public class AccountConfiguration
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public StorageType Type { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Secret { get; set; }

        public string DisplayName
        {
            get { return string.Format("{0} ({1})", Name, Type); }
        }
    }
}