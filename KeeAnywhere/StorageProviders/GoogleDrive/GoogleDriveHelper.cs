using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using IdentityModel;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;

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

        internal const string Authority = "https://accounts.google.com";

        internal static string[] Scopes = {
            OidcConstants.StandardScopes.OpenId,
            OidcConstants.StandardScopes.Profile,
            DriveService.Scope.Drive };

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

        public static OidcFlow CreateOidcFlow()
        {
            return new OidcFlow(StorageType.GoogleDrive, Authority, GoogleDriveClientId, GoogleDriveClientSecret, Scopes);
        }

        public static async Task<DriveService> GetClient(AccountConfiguration account)
        {
            return await GetClient(new TokenResponse { RefreshToken = account.Secret });
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
            client.HttpClient.Timeout = Timeout.InfiniteTimeSpan;

            return client;
        }

        public static async Task<File> GetFileByPath(this DriveService api, string path, bool resolveFinalShortcut)
        {
            var parts = path.Split('/');
            File file = null;
			var partsLength = parts.Count();
			for (int i = 0; i < partsLength; i++)
            {
				var part = parts[i];
                var query = api.Files.List();
                query.Q = string.Format("name = '{0}' and '{1}' in parents and trashed = false", part,
                //query.Q = string.Format("title = '{0}'", part,
                    file == null ? "root" : file.Id);

                var result = await query.ExecuteAsync();

                file = result.Files.FirstOrDefault();
                if (file == null) return null;
				if (resolveFinalShortcut || i != partsLength-1)
				{
					if (file.MimeType == "application/vnd.google-apps.shortcut")
					{
						if (file.ShortcutDetails == null)
						{
							var fileQuery = api.Files.Get(file.Id);
							fileQuery.Fields = "shortcutDetails";
							file = await fileQuery.ExecuteAsync();
						}
						file = await api.Files.Get(file.ShortcutDetails.TargetId).ExecuteAsync();
					}
				}
            }

            return file;
        }
    }
}
