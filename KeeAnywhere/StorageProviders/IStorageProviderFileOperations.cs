using System.IO;
using System.Threading.Tasks;

namespace KeeAnywhere.StorageProviders
{
    public interface IStorageProviderFileOperations
    {
        // File operations
        Task<Stream> Load(string path);
        Task Save(Stream stream, string path);
        Task Copy(string sourcePath, string destPath);
        Task Delete(string path);

        // Validation operations
        bool IsFilenameValid(string filename);
    }
}