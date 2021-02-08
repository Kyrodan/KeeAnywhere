using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api;

namespace KeeAnywhere.StorageProviders.Dropbox
{
    public class DropboxHelper
    {
        /*
        The consumer key and the secret key included here are dummy keys.
        You should go to https://dropbox.com/developers/apps to create your own application
        and get your own keys.

        This is done to prevent bots from scraping the keys from the source code posted on the web.

        Every now and then an accidental checkin of keys may occur, but these are all dummy applications
        created specifically for development that are deleted frequently and limited to the developer,
        never the real production keys.
       */

        //TODO: Change API keys!!!
        internal const string DropboxFullAccessClientId = "dummy";
        internal const string DropboxFullAccessClientSecret = "dummy";
        internal const string DropboxAppFolderOnlyClientId = "dummy";
        internal const string DropboxAppFolderOnlyClientSecret = "dummy";

        public static DropboxClient GetApi(bool isRestricted, string refreshToken)
        {
            var appKey = isRestricted ? DropboxAppFolderOnlyClientId : DropboxFullAccessClientId;
            var appSecret = isRestricted ? DropboxAppFolderOnlyClientSecret : DropboxFullAccessClientSecret;

            var api = new DropboxClient(refreshToken, appKey, appSecret, GetConfig());

            return api;
        }

        public static DropboxClient GetApi(string accessToken)
        {
            var api = new DropboxClient(accessToken, GetConfig());

            return api;
        }

        private static DropboxClientConfig GetConfig()
        {
            return new DropboxClientConfig
            {
                HttpClient = ProxyTools.CreateHttpClient()
            };
        }

    }
}
