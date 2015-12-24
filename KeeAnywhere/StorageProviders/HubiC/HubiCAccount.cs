using System.Runtime.Serialization;

namespace KeeAnywhere.StorageProviders.HubiC
{
    [DataContract]
    internal class HubiCAccount
    {
        [DataMember(Name="email")]
        public string EMail { get; set; }

        [DataMember(Name = "firstname")]
        public string FirstName { get; set; }

        [DataMember(Name = "lastname")]
        public string LastName { get; set; }
    }
}