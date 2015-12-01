using System.IO;

namespace KeeAnywhere.StorageProviders
{
    public interface IStorageProvider
    {
        bool Delete(string path);
        Stream Load(string path);
        bool Save(MemoryStream stream, string path);
        bool Move(string pathFrom, string pathTo);
    }
}