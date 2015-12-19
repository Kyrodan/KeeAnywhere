using System;

namespace KeeAnywhere.StorageProviders
{
    public class StorageProviderItem
    {
        public StorageProviderItemType Type { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset? LastModifiedDateTime { get; set; }
        public string ParentReferenceId { get; set; }

    }
}