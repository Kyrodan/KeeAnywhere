using System.Runtime.Serialization;

namespace KeeAnywhere.StorageProviders.HubiC
{
    [DataContract]
    public class SwiftContainer
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }

        [DataMember(Name = "bytes")]
        public ulong Bytes { get; set; }
    }
}