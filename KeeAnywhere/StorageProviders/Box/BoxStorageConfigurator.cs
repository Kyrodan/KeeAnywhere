using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Box.V2;
using Box.V2.Auth;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;

namespace KeeAnywhere.StorageProviders.Box
{
    // OAuth Flow with localhost listener. Box does not allow "insecure" http-Access in general.
    // => can't user local server here.

    //public class BoxStorageConfigurator : IStorageConfigurator
    //{
    //    public async Task<AccountConfiguration> CreateAccount()
    //    {
    //        var f = new OidcWaitForm();
    //        f.InitEx(StorageType.Box);
    //        f.Show();


    //        var clientId = BoxHelper.Config.ClientId;
    //        var clientSecret = BoxHelper.Config.ClientSecret;

    //        var browser = new OidcSystemBrowser();

    //        var redirectUri = browser.RedirectUri;

    //        var config = new BoxConfig(clientId, clientSecret, new Uri(redirectUri));

    //        var uri = config.AuthCodeUri;
    //        var query = await browser.GetQueryStringAsync(uri.ToString(), f.CancellationToken);

    //        var code = query["code"];

    //        var api = BoxHelper.GetClient();
    //        var token = await api.Auth.AuthenticateAsync(code);

    //        if (token == null || token.RefreshToken == null || token.AccessToken == null)
    //        {
    //            throw new Exception("Unauthorized");
    //        }

    //        var user = await api.UsersManager.GetCurrentUserInformationAsync();

    //        f.Close();

    //        return new AccountConfiguration
    //        {
    //            Type = StorageType.Box,
    //            Id = user.Id,
    //            Name = user.Name,
    //            Secret = token.RefreshToken
    //        };
    //    }
    //}

    public class BoxStorageConfigurator : IStorageConfigurator, IOAuth2Provider
    {
        private OAuthSession _token;
        private BoxClient _api;

        public async Task<AccountConfiguration> CreateAccount()
        {
            var isOk = OAuth2Flow.TryAuthenticate(this);
            if (!isOk) return null;

            var user = await _api.UsersManager.GetCurrentUserInformationAsync();

            return new AccountConfiguration
            {
                Type = StorageType.Box,
                Id = user.Id,
                Name = user.Name,
                Secret = _token.RefreshToken
            };
        }

        public async Task Initialize()
        {
            this.RedirectionUrl = BoxHelper.Config.RedirectUri;
            this.AuthorizationUrl = BoxHelper.Config.AuthCodeUri;
        }

        public bool CanClaim(Uri uri, string documentTitle)
        {
            return uri.ToString().StartsWith(this.RedirectionUrl.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> Claim(Uri uri, string documentTitle)
        {
            IDictionary<string, string> keyDictionary = new Dictionary<string, string>();
            var qSplit = uri.Query.Split('?');
            foreach (var kvp in qSplit[qSplit.Length - 1].Split('&'))
            {
                var kvpSplit = kvp.Split('=');
                if (kvpSplit.Length == 2)
                {
                    keyDictionary.Add(kvpSplit[0], kvpSplit[1]);
                }
            }

            if (!keyDictionary.ContainsKey("code"))
                return false;

            var authCode = keyDictionary["code"];
            if (string.IsNullOrEmpty(authCode))
                return false;

            _api = BoxHelper.GetClient();
            _token = await _api.Auth.AuthenticateAsync(authCode);

            return _token != null && _token.RefreshToken != null && _token.AccessToken != null;
        }

        public Uri PreAuthorizationUrl { get; protected set; }
        public Uri AuthorizationUrl { get; protected set; }
        public Uri RedirectionUrl { get; protected set; }
        public string FriendlyProviderName { get { return "Box"; } }
    }
}