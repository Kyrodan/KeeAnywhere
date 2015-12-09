using System;
using System.Net;
using KeeAnywhere.StorageProviders;

namespace KeeAnywhere.WebRequest
{
    public sealed class KeeAnywhereWebRequestCreator : IWebRequestCreate
    {
        private readonly StorageService _storageService;

        public KeeAnywhereWebRequestCreator(StorageService storageService)
        {
            if (storageService == null) throw new ArgumentNullException("storageService");
            _storageService = storageService;
        }

        public System.Net.WebRequest Create(Uri uri)
        {
            var providerUri = new StorageUri(uri);
            var provider = _storageService.GetProviderByUri(providerUri);
            var itemPath = providerUri.GetPath();

            return new KeeAnywhereWebRequest(provider, itemPath);
        }

    }
}
