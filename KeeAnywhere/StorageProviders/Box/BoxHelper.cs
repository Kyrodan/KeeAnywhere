using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box.V2;
using Box.V2.Auth;
using Box.V2.Config;
using Box.V2.Converter;
using Box.V2.Models;
using Box.V2.Services;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.Box
{
    public static class BoxHelper
    {
        /*
               The consumer key and the secret key included here are dummy keys.
               You should go to https://developer.box.com/ to create your own application
               and get your own keys.

               This is done to prevent bots from scraping the keys from the source code posted on the web.

               Every now and then an accidental checkin of keys may occur, but these are all dummy applications
               created specifically for development that are deleted frequently and limited to the developer,
               never the real production keys.
              */

        //TODO: Change API keys!!!
        internal const string BoxClientId = "dummy";
        internal const string BoxClientSecret = "dummy";


        internal const string RedirectUri = "https://localhost/oauth";
        internal const int Limit = 500;

        internal static BoxConfig Config;

        private static readonly IDictionary<string, BoxClient> Cache = new Dictionary<string, BoxClient>();

        static BoxHelper()
        {
            Config = new BoxConfig(BoxClientId, BoxClientSecret, new Uri(RedirectUri));
        }

        public static async Task<BoxClient> GetClient(AccountConfiguration account)
        {
            if (Cache.ContainsKey(account.Id)) return Cache[account.Id];

            var session = new OAuthSession(null, account.Secret, 0, "bearer");

            var client = GetClient(session);
            client.Auth.SessionAuthenticated += (sender, args) => account.Secret = args.Session.RefreshToken;

            session = await client.Auth.RefreshAccessTokenAsync(account.Secret);
            Cache.Add(account.Id, client);

            return client;
        }

        public static BoxClient GetClient()
        {
            return GetClient((OAuthSession)null);
        }

        public static BoxClient GetClient(OAuthSession session)
        {
            var handler = new BoxHttpRequestHandler();
            var converter = new BoxJsonConverter();
            var service = new BoxService(handler);
            var authRepository = new AuthRepository(Config, service, converter, session);

            var client = new BoxClient(Config, converter, handler, service, authRepository);
            return client;
        }

        public static async Task<BoxItem> GetFileByPath(this BoxClient api, string path)
        {
            var parts = path.Split('/');
            BoxItem item = null;

            foreach (var part in parts)
            {
                var items = await api.FoldersManager.GetFolderItemsAsync(item == null ? "0" : item.Id, Limit);
                item = items.Entries.FirstOrDefault(_ => _.Name == part);

                if (item == null) return null;
            }

            return item;
        }
    }
}
