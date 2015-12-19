using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.GoogleDrive
{
    static class GoogleDriveHelper
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

        public static async Task<DriveService> GetClient(AccountConfiguration account)
        {
            var credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets()
                {
                    ClientId = GoogleDriveClientId,
                    ClientSecret = GoogleDriveClientSecret,
                },
                new []{ DriveService.Scope.Drive }, 
                account.Id, 
                CancellationToken.None,
                new AccountDataStore(account)
                );

            var initializer = new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = "KeeAnywhere",
            };

            var client = new DriveService(initializer);

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

    public class AccountDataStore : IDataStore
    {
        private readonly AccountConfiguration _account;
        private TokenResponse _token;

        public AccountDataStore(AccountConfiguration account)
        {
            if (account == null) throw new ArgumentNullException("account");
            _account = account;
        }

        public async Task StoreAsync<T>(string key, T value)
        {
            _token = value as TokenResponse;
            if (_token != null)
            {
                _account.Secret = _token.RefreshToken;
            }

            //return TaskEx.Delay(0);
        }

        public async Task DeleteAsync<T>(string key)
        {
            _account.Secret = null;
            //return TaskEx.Delay(0);
        }

        public Task<T> GetAsync<T>(string key)
        {
            var completionSource = new TaskCompletionSource<T>();

            var i = Activator.CreateInstance<T>();
            var token = i as TokenResponse;
            if (token == null)
            {
                completionSource.SetResult(default(T));
                return completionSource.Task;
            }

            token.RefreshToken = _account.Secret;

            completionSource.SetResult(i);

            return completionSource.Task;
        }

        public async Task ClearAsync()
        {
            _account.Secret = null;
            // return TaskEx.Delay(0);
        }
    }
}
