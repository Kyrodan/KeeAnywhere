using System;
using System.Linq;

namespace KeeAnywhere.StorageProviders
{
    public static class StorageProviderExtensions
    {
        public static bool IsCompletePathValid(this IStorageProvider provider, string path)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            if (path == null) return false;

            var parts = path.Split('/');

            return parts.All(provider.IsFilenameValid);
        }
    }
}