using System.Runtime.Serialization;
using KeeAnywhere.StorageProviders;

namespace KeeAnywhere.Configuration
{
    public class AccountIdentifier
    {
        [DataMember]
        public StorageType Type { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}