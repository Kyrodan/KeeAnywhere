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

        internal const string OneDriveClientId = ApiKeys.OneDriveClientId;

        internal const string Authority = "https://login.microsoftonline.com/consumers/v2.0";

        public static readonly string[] Scopes = {
            OidcConstants.StandardScopes.OpenId,
            OidcConstants.StandardScopes.Profile,
            "offline_access",
            "Files.ReadWrite"
        };

        // OneDrive (restricted): access to the app folder Apps/KeeAnywhere only (#391)
        public static readonly string[] ScopesRestricted = {
            OidcConstants.StandardScopes.OpenId,
            OidcConstants.StandardScopes.Profile,
            "offline_access",
            "Files.ReadWrite.AppFolder"
        };

        private static readonly IDictionary<string, GraphServiceClient> Cache = new Dictionary<string, GraphServiceClient>();

        public static OidcFlow CreateOidcFlow(bool isAccessRestricted)
        {
            return new OidcFlow(
                isAccessRestricted ? StorageType.OneDriveRestricted : StorageType.OneDrive,
                Authority, OneDriveClientId, null,
                isAccessRestricted ? ScopesRestricted : Scopes)
            {
                PrepareLoginRequest = request => { request.FrontChannelExtraParameters.Add("prompt", "select_account"); } // login
            };
        }

        public static GraphServiceClient GetApi(AccountConfiguration account)
        {
            // the same Microsoft account can be added as OneDrive and as OneDrive (restricted)
            var key = GetCacheKey(account.Type, account.Id);
            if (Cache.ContainsKey(key)) return Cache[key];

            var isAccessRestricted = account.Type == StorageType.OneDriveRestricted;
            var authProvider = new OneDriveAuthenticationProvider(CreateOidcFlow(isAccessRestricted), account);

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
            if (isAccessRestricted) api.UseAppFolderAsRoot();

            Cache.Add(key, api);

            return api;
        }

        public static void InvalidateCache(string accountId)
        {
            Cache.Remove(GetCacheKey(StorageType.OneDrive, accountId));
            Cache.Remove(GetCacheKey(StorageType.OneDriveRestricted, accountId));
        }

        private static string GetCacheKey(StorageType type, string accountId)
        {
            return type + "/" + accountId;
        }
    }
}