using Microsoft.OneDrive.Sdk;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public static class OneDriveApiExtensions
    {
        public static bool IsFolder(this Item item)
        {
            return item.Folder != null;
        }

        public static bool IsFile(this Item item)
        {
            return item.File != null;
        }
    }
}