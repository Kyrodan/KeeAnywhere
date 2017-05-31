using System;

namespace KeeAnywhere.StorageProviders.AmazonS3
{
    internal static class AmazonS3StringExtensions
    {
        internal static string RemovePrefix(this string s, string prefix)
        {
            if (string.IsNullOrEmpty(s)) throw new ArgumentNullException("s");

            if (string.IsNullOrEmpty(prefix))
                return s;

            return s.Substring(prefix.Length);
        }

        internal static string RemoveTrailingSlash(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            return s.EndsWith("/") ? s.Remove(s.Length - 1) : s;
        }
    }
}