using System.Runtime.Serialization;
using KeeAnywhere.StorageProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KeeAnywhere.Configuration
{
    public class AccountIdentifier
    {
        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public StorageType Type { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}