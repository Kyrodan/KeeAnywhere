using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using IdentityModel;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;
using Microsoft.Graph;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveHelper
    {
        /*
         The consumer key and the secret key included here are dummy keys.
         You should go to https://docs.microsoft.com/graph/auth-register-app-v2?view=graph-rest-1.0 to create your own application
         and get your own keys.

         This is done to prevent bots from scraping the keys from the source code posted on the web.

         Every now and then an accidental checkin of keys may occur, but these are all dummy applications
         created specifically for development that are deleted frequently and limited to the developer,
         never the real production keys.
        */

        //TODO: Change API keys!!!
        internal const string OneDriveClientId = "dummy";

        internal const string Authority = "https://login.microsoftonline.com/consumers/v2.0";

        public static readonly string[] Scopes = {
            OidcConstants.StandardScopes.OpenId,
            OidcConstants.StandardScopes.Profile,
            "offline_access",
            "Files.ReadWrite"
        };

        private static readonly IDictionary<string, GraphServiceClient> Cache = new Dictionary<string, GraphServiceClient>();

        public static OidcFlow CreateOidcFlow()
        {
            return new OidcFlow(StorageType.OneDrive, Authority, OneDriveClientId, null, Scopes)
            {
                PrepareLoginRequest = request => { request.FrontChannelExtraParameters.Add("prompt", "select_account"); } // login
            };
        }

        public static GraphServiceClient GetApi(AccountConfiguration account)
        {
            if (Cache.ContainsKey(account.Id)) return Cache[account.Id];

            var authProvider = new OneDriveAuthenticationProvider(CreateOidcFlow(), account.Secret);

            //var httpProvider = new HttpProvider(ProxyTools.CreateHttpClientHandler(), true)
            //{
            //    OverallTimeout = Timeout.InfiniteTimeSpan
            //};

            //var api = new GraphServiceClient(authProvider, httpProvider);
            //var api = new GraphServiceClient(ProxyTools.CreateHttpClient(), authProvider);
            //var api = new GraphServiceClient(authProvider);

            var handlers = GraphClientFactory.CreateDefaultHandlers();
            var httpClient = GraphClientFactory.Create(handlers, "v1.0", "Global", ProxyTools.GetProxy());
            var api = new GraphServiceClient(httpClient, authProvider);

            Cache.Add(account.Id, api);

            return api;
        }
    }
}