using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using Microsoft.Graph;
using Microsoft.OneDrive.Sdk.Authentication;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveHelper
    {
        /*
         The consumer key and the secret key included here are dummy keys.
         You should go to https://dev.onedrive.com/ to create your own application
         and get your own keys.

         This is done to prevent bots from scraping the keys from the source code posted on the web.

         Every now and then an accidental checkin of keys may occur, but these are all dummy applications
         created specifically for development that are deleted frequently and limited to the developer,
         never the real production keys.
        */

        //TODO: Change API keys!!!
        internal const string OneDriveClientId = "dummy";
        internal const string OneDriveClientSecret = "dummy";


        //private static readonly string[] Scopes = new[] {"wl.offline_access", "wl.skydrive_update"};
        public static readonly string[] Scopes = { "offline_access", "onedrive.readwrite" };
        public static readonly string ApiUrl = "https://api.onedrive.com/v1.0";
        public static readonly string RedirectionUrl = "https://login.live.com/oauth20_desktop.srf";
        public static readonly string SignOutUrl = "https://login.live.com/oauth20_logout.srf";

        private static readonly IDictionary<string, IGraphServiceClient> Cache = new Dictionary<string, IGraphServiceClient>();


        public static async Task<IGraphServiceClient> GetApi(AccountConfiguration account)
        {
            if (Cache.ContainsKey(account.Id)) return Cache[account.Id];
       
            var authProvider = new OneDriveAuthenticationProvider(
                                                    OneDriveClientId,
                                                    OneDriveClientSecret);

            await authProvider.AuthenticateByRefreshTokenAsync(account.Secret);

            var httpProvider = new HttpProvider(ProxyTools.CreateHttpClientHandler(), true) {
                OverallTimeout = Timeout.InfiniteTimeSpan
            };

            var api = new GraphServiceClient(ApiUrl, authProvider, httpProvider);
            Cache.Add(account.Id, api);
            
            return api;
        }

        public static async Task<IGraphServiceClient> GetApi(AccountSession accountSession)
        {
            var authProvider = new OneDriveAuthenticationProvider(
                                                    OneDriveClientId,
                                                    OneDriveClientSecret);

            await authProvider.AuthenticateByAccountSessionAsync(accountSession);

            var httpProvider = new HttpProvider(ProxyTools.CreateHttpClientHandler(), true);
            var api = new GraphServiceClient(ApiUrl, authProvider, httpProvider);

            return api;
        }
    }
}