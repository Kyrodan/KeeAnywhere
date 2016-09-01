using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Http;
using Google.Apis.Services;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.GoogleDrive
{
    public static class GoogleDriveHelper
    {
        /*
          The consumer key and the secret key included here are dummy keys.
          You should go to https://console.developers.google.com/ to create your own application
          and get your own keys.

          This is done to prevent bots from scraping the keys from the source code posted on the web.

          Every now and then an accidental checkin of keys may occur, but these are all dummy applications
          created specifically for development that are deleted frequently and limited to the developer,
          never the real production keys.
        */

        //TODO: Change API keys!!!
        internal const string GoogleDriveClientId = "dummy";
        internal const string GoogleDriveClientSecret = "dummy";


        internal const string RedirectUri = "urn:ietf:wg:oauth:2.0:oob:auto";
        internal const string LogoutUri = "https://accounts.google.com/Logout?continue={0}";
        internal static string[] Scopes = { DriveService.Scope.Drive };
        internal static IAuthorizationCodeFlow AuthFlow;

        static GoogleDriveHelper()
        {
            var flowInitializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = GoogleDriveClientId,
                    ClientSecret = GoogleDriveClientSecret
                },
                Scopes = Scopes,
                HttpClientFactory = new GoogleDriveHttpClientFactory()
            };

            AuthFlow = new GoogleAuthorizationCodeFlow(flowInitializer);
        }

        public static async Task<DriveService> GetClient(AccountConfiguration account)
        {
            return await GetClient(new TokenResponse { RefreshToken = account.Secret } );
        }

        public static async Task<DriveService> GetClient(TokenResponse token)
        {
            var credentials = new UserCredential(
               AuthFlow,
               null,
               token);

            var driveInitializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = credentials,
                HttpClientFactory = new GoogleDriveHttpClientFactory(),
                ApplicationName = "KeeAnywhere",
            };

            var client = new DriveService(driveInitializer);

            return client;
        }

        public static async Task<File> GetFileByPath(this DriveService api, string path)
        {
            var parts = path.Split('/');
            File file = null;

            foreach (var part in parts)
            {
                var query = api.Files.List();
                query.Q = string.Format("name = '{0}' and '{1}' in parents and trashed = false", part,
                //query.Q = string.Format("title = '{0}'", part,
                    file == null ? "root" : file.Id);

                var result = await query.ExecuteAsync();

                file = result.Files.FirstOrDefault();
                if (file == null) return null;
            }

            return file;
        }
    }
}
