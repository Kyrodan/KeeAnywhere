using System.Net.Http;
using Google.Apis.Http;

namespace KeeAnywhere.StorageProviders.GoogleDrive
{
    public class GoogleDriveHttpClientFactory : HttpClientFactory
    {
        protected override HttpMessageHandler CreateHandler(CreateHttpClientArgs args)
        {
            var handler = base.CreateHandler(args);

            var clientHandler = handler as HttpClientHandler;

            if (clientHandler != null)
                clientHandler.ApplyProxy();

            return handler;
        }
    }
}