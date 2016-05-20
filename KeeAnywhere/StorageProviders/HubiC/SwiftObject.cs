using System;
using System.Runtime.Serialization;

namespace KeeAnywhere.StorageProviders.HubiC
{
    [DataContract]
    public class SwiftObject
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "content_type")]
        public string ContentType { get; set; }

        [DataMember(Name = "bytes")]
        public ulong Bytes { get; set; }

        [DataMember(Name = "last_modified")]
        public DateTime LastModified { get; set; }

        [DataMember(Name = "hash")]
        public string Hash { get; set; }

        [DataMember(Name = "subdir")]
        public string VirtualSubDirectory { get; set; }
    }
}