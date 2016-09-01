using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using Kyrodan.HiDrive;
using Kyrodan.HiDrive.Authentication;

namespace KeeAnywhere.StorageProviders.HiDrive
{
    public static class HiDriveHelper
    {
        /*
            The consumer key and the secret key included here are dummy keys.
            You should go to https://dev.strato.com/hidrive/get_key to create your own application
            and get your own keys.

            This is done to prevent bots from scraping the keys from the source code posted on the web.

            Every now and then an accidental checkin of keys may occur, but these are all dummy applications
            created specifically for development that are deleted frequently and limited to the developer,
            never the real production keys.
        */

        //TODO: Change API keys!!!
        internal const string HiDriveClientId = "dummy";
        internal const string HiDriveClientSecret = "dummy";

        public const string RedirectUri = "http://localhost";

        public static IHiDriveAuthenticator GetAuthenticator()
        {
            return new HiDriveAuthenticator(HiDriveClientId, HiDriveClientSecret, ProxyTools.CreateHttpClientHandler);
        }

        internal static IHiDriveClient GetClient(IHiDriveAuthenticator authenticator)
        {
            return new HiDriveClient(authenticator, ProxyTools.CreateHttpClientHandler);
        }

        public static async Task<IHiDriveClient> GetClient(AccountConfiguration account)
        {
            return await GetClient(account.Secret);
        }

        public static async Task<IHiDriveClient> GetClient(string refreshToken)
        {
            var authenticator = GetAuthenticator();
            await authenticator.AuthenticateByRefreshTokenAsync(refreshToken);

            var client = GetClient(authenticator);

            return client;
        }
    }
}
