using System.IO;

namespace KeeAnywhere.Offline
{
    public struct CacheFileInfo
    {
        public CacheFileInfo(Stream stream, string hash)
        {
            Stream = stream;
            Hash = hash;
        }

        public string Hash { get; set; }
        public Stream Stream { get; set; }
    }
}