using System;
using System.Threading.Tasks;
using db = Dropbox.Api;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;

namespace KeeAnywhere.StorageProviders.Dropbox
{
    public class DropboxStorageConfigurator : IStorageConfigurator
    {
        private readonly bool _isAccessRestricted;

        public DropboxStorageConfigurator(bool isAccessRestricted)
        {
            this._isAccessRestricted = isAccessRestricted;
        }

        public async Task<AccountConfiguration> CreateAccount()
        {
            var f = new OidcWaitForm();
            f.InitEx(_isAccessRestricted ? StorageType.DropboxRestricted : StorageType.Dropbox);
            f.Show();

            var clientId = _isAccessRestricted ? DropboxHelper.DropboxAppFolderOnlyClientId : DropboxHelper.DropboxFullAccessClientId;
            var clientSecret = _isAccessRestricted ? DropboxHelper.DropboxAppFolderOnlyClientSecret : DropboxHelper.DropboxFullAccessClientSecret;

            string[] scopes =
            {
                "account_info.read",
                "files.metadata.write",
                "files.metadata.read",
                "files.content.write",
                "files.content.read"
            };

            var browser = new OidcSystemBrowser(50001, 50005);

            var redirectUri = browser.RedirectUri;
            var state = Guid.NewGuid().ToString("N");
            var codeVerifier = db.DropboxOAuth2Helper.GeneratePKCECodeVerifier();
            var codeChallenge = db.DropboxOAuth2Helper.GeneratePKCECodeChallenge(codeVerifier);
            var uri = db.DropboxOAuth2Helper.GetAuthorizeUri(db.OAuthResponseType.Code, clientId, redirectUri, state, false, false, null, false, db.TokenAccessType.Offline, scopes, db.IncludeGrantedScopes.None, codeChallenge);

            var query = await browser.GetQueryStringAsync(uri.ToString(), f.CancellationToken);


            var resultState = query["state"];

            if (state != resultState)
            {
                throw new Exception("MiM-Attack?");
            }

            var code = query["code"];

            var response = await db.DropboxOAuth2Helper.ProcessCodeFlowAsync(code, clientId, null, redirectUri, null, codeVerifier);

            var api = DropboxHelper.GetApi(response.AccessToken);
            var owner = await api.Users.GetCurrentAccountAsync();

            var account = new AccountConfiguration()
            {
                Id = owner.AccountId,
                Name = owner.Name.DisplayName,
                Type = _isAccessRestricted ? StorageType.DropboxRestricted : StorageType.Dropbox,
                Secret = response.RefreshToken,
            };

            f.Close();

            return account;

        }
    }

    //public class DropboxStorageConfigurator : IStorageConfigurator, IOAuth2Provider
    //{
    //    private readonly bool _isAccessRestricted;

    //    private string _state;
    //    private OAuth2Response _oauthResponse;

    //    public DropboxStorageConfigurator(bool isAccessRestricted)
    //    {
    //        this._isAccessRestricted = isAccessRestricted;
    //    }

    //    public async Task<AccountConfiguration> CreateAccount()
    //    {
    //        var isOk = OAuth2Flow.TryAuthenticate(this);

    //        if (!isOk) return null;

    //        var api = DropboxHelper.GetApi(_oauthResponse.AccessToken);
    //        var owner = await api.Users.GetCurrentAccountAsync();

    //        var account = new AccountConfiguration()
    //        {
    //            Id = owner.AccountId,
    //            Name = owner.Name.DisplayName,
    //            Type = _isAccessRestricted ? StorageType.DropboxRestricted : StorageType.Dropbox,
    //            Secret = _oauthResponse.AccessToken,
    //        };

    //        return account;
    //    }

    //    public async Task Initialize()
    //    {
    //        _state = Guid.NewGuid().ToString("N");
    //        this.AuthorizationUrl = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token,
    //            _isAccessRestricted ? DropboxHelper.DropboxAppFolderOnlyClientId : DropboxHelper.DropboxFullAccessClientId, RedirectionUrl, _state);
    //    }

    //    public bool CanClaim(Uri uri, string documentTitle)
    //    {
    //        return uri.ToString().StartsWith(this.RedirectionUrl.ToString(), StringComparison.OrdinalIgnoreCase);
    //    }

    //    public Task<bool> Claim(Uri uri, string documentTitle)
    //    {
    //        var cs = new TaskCompletionSource<bool>();

    //        try
    //        {
    //            var result = DropboxOAuth2Helper.ParseTokenFragment(uri);
    //            if (result.State != _state)
    //            {
    //                // The state in the response doesn't match the state in the request.
    //                cs.SetResult(false);
    //                return cs.Task;
    //            }

    //            _oauthResponse = result;
    //            cs.SetResult(true);
    //            return cs.Task;
    //        }
    //        catch (Exception ex)
    //        {
    //            cs.SetException(ex);
    //            return cs.Task;
    //        }
    //    }

    //    public Uri PreAuthorizationUrl { get { return new Uri("https://www.dropbox.com/logout"); }  }

    //    public Uri AuthorizationUrl { get; protected set; }
    //    //public Uri RedirectionUrl { get { return new Uri("http://localhost/auth_redirection"); } }
    //    public Uri RedirectionUrl { get { return new Uri("https://www.dropbox.com/1/oauth2/redirect_receiver"); } }
    //    public string FriendlyProviderName { get { return "Dropbox"; } }
    //}
}