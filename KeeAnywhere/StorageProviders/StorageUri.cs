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
            var segments = this.Segments;
            if (segments.Length < 3)
                return null;

            var account = UnescapeDataString(segments[1]);
            account = StripSlashes(account);
            return account;
        }

        public string GetPath()
        {
            var segments = this.Segments;
            if (segments.Length < 3)
                return null;

            segments = segments.Where((val, idx) => idx >= 2).ToArray();

            var path = string.Concat(segments);
            path = UnescapeDataString(path);
            return path;
        }

        private string StripSlashes(string s)
        {
            return s.Replace("/", string.Empty);
        }
    }
}
