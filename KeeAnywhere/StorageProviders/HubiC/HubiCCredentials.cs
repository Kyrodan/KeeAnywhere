using System;
using System.Runtime.Serialization;

namespace KeeAnywhere.StorageProviders.HubiC
{
    [DataContract]
    public class HubiCCredentials
    {
        [DataMember(Name = "token")]
        public string Token { get; set; }

        [DataMember(Name = "endpoint")]
        public string Endpoint { get; set; }

        [DataMember(Name = "expires")]
        public DateTime Exprires { get; set; }
    }
}