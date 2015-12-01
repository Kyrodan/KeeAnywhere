using KoenZomers.OneDrive.Api.Entities;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public static class OneDriveApiExtensions
    {
        public static bool IsFolder(this OneDriveItem item)
        {
            return item.Folder != null;
        }

        public static bool IsFile(this OneDriveItem item)
        {
            return item.File != null;
        }
    }
}