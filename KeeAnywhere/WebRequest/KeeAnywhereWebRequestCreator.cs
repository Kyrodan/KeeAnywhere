using System;
using System.Net;
using KeeAnywhere.StorageProviders;

namespace KeeAnywhere.WebRequest
{
    public sealed class KeeAnywhereWebRequestCreator : IWebRequestCreate
    {
        private readonly StorageProviderService _storageProviderService;

        public KeeAnywhereWebRequestCreator(StorageProviderService storageProviderService)
        {
            if (storageProviderService == null) throw new ArgumentNullException("storageProviderService");
            _storageProviderService = storageProviderService;
        }

        public System.Net.WebRequest Create(Uri uri)
        {
            var providerUri = new StorageProviderUri(uri);
            var provider = _storageProviderService.GetByUri(providerUri);
            var itemPath = providerUri.GetPath();

            //var accountName = odUri.GetAccountName();

            //if (itemPath == null) throw new Exception();
            //if (accountName == null) throw new Exception();

            //var account = _configService.Accounts.SingleOrDefault(_ => _.Name == accountName);

            //if (account == null)
            //    throw new InvalidOperationException(string.Format("Account '{0}' not found.", accountName));

            //var api = Task.Run(async () => await _oneDriveService.TryGetApi(account));

            //if (api.Result == null)
            //    throw new Exception();

            //var provider = new OneDriveStorageProvider(api.Result);


            return new KeeAnywhereWebRequest(provider, itemPath);
        }

    }
}
