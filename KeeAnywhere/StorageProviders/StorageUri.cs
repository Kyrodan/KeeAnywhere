using System;
using System.Linq;

namespace KeeAnywhere.StorageProviders
{
    public class StorageUri : Uri
    {
        public static string BuildUriString(StorageType type, string accountName, string path)
        {
            if (accountName == null) throw new ArgumentNullException("accountName");
            if (path == null) throw new ArgumentNullException("path");

            var scheme = StorageRegistry.Descriptors.First(_ => _.Type == type).Scheme;

            return string.Format("{0}:///{1}/{2}", scheme, accountName, path);
        }

        public StorageUri(string uriString) : base(uriString)
        {
            if (uriString == null) throw new ArgumentNullException("uriString");
        }

        public StorageUri(Uri uri) : this(uri.OriginalString)
        {
        }

        public string GetAccountName()
        {
            var segments = this.OriginalString.Split('/');
            if (segments.Length < 4)
                return null;

            var account = UnescapeDataString(segments[3]);
            return account;
        }

        public string GetPath()
        {
            var segments = this.OriginalString.Split('/');
            if (segments.Length < 5)
                return null;

            segments = segments.Where((val, idx) => idx >= 4).ToArray();

            var path = string.Join("/", segments);
            path = UnescapeDataString(path);
            return path;
        }
    }
}
