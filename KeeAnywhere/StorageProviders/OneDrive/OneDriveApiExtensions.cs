using Microsoft.Graph;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public static class OneDriveApiExtensions
    {
        public static bool IsFolder(this DriveItem item)
        {
            return item.Folder != null;
        }

        public static bool IsFile(this DriveItem item)
        {
            return item.File != null;
        }
    }
}