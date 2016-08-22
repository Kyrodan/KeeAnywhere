using System.Collections.Generic;
using System.Runtime.Serialization;
using KeeAnywhere.Json;
using KeeAnywhere.StorageProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KeeAnywhere.Configuration
{
    [DataContract]
    public class AccountConfiguration
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public StorageType Type { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        [JsonEncrypt]
        public string Secret { get; set; }

        [DataMember]
        public Dictionary<string, string> AdditionalSettings { get; set; }

        public string DisplayName
        {
            get { return string.Format("{0} ({1})", Name, Type); }
        }

        public AccountIdentifier GetAccountIdentifier()
        {
            return new AccountIdentifier {Name = this.Name, Type = this.Type};
        }
    }
}