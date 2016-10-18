using System;
using System.IO;
using System.Threading.Tasks;

namespace KeeAnywhere.StorageProviders
{
    public abstract class FileOperationsProxyProvider : IStorageProviderFileOperations
    {
        protected IStorageProvider BaseProvider { get; private set; }

        protected FileOperationsProxyProvider(IStorageProvider baseProvider)
        {
            if (baseProvider == null) throw new ArgumentNullException("baseProvider");
            BaseProvider = baseProvider;
        }

        public virtual Task<Stream> Load(string path)
        {
            return BaseProvider.Load(path);
        }

        public virtual Task Save(Stream stream, string path)
        {
            return BaseProvider.Save(stream, path);
        }

        public virtual Task Copy(string sourcePath, string destPath)
        {
            return BaseProvider.Copy(sourcePath, destPath);
        }

        public virtual Task Delete(string path)
        {
            return BaseProvider.Delete(path);
        }

        public virtual bool IsFilenameValid(string filename)
        {
            return BaseProvider.IsFilenameValid(filename);
        }
    }
}