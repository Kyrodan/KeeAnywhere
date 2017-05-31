using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KeeAnywhere.StorageProviders
{
    public abstract class ProxyProvider : IStorageProvider
    {
        protected IStorageProvider BaseProvider { get; private set; }

        protected ProxyProvider(IStorageProvider baseProvider)
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

        public virtual Task<StorageProviderItem> GetRootItem()
        {
            return BaseProvider.GetRootItem();
        }

        public virtual Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent)
        {
            return BaseProvider.GetChildrenByParentItem(parent);
        }

        public virtual Task<IEnumerable<StorageProviderItem>> GetChildrenByParentPath(string path)
        {
            return BaseProvider.GetChildrenByParentPath(path);
        }
    }
}