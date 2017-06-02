using System;
using System.Threading.Tasks;
using Dropbox.Api;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;

namespace KeeAnywhere.StorageProviders.Dropbox
{
    public class DropboxStorageConfigurator : IStorageConfigurator, IOAuth2Provider
    {
        private readonly bool _isAccessRestricted;

        private string _state;
        private OAuth2Response _oauthResponse;

        public DropboxStorageConfigurator(bool isAccessRestricted)
        {
            this._isAccessRestricted = isAccessRestricted;
        }

        public async Task<AccountConfiguration> CreateAccount()
        {
            var isOk = OAuth2Flow.TryAuthenticate(this);

            if (!isOk) return null;

            var api = DropboxHelper.GetApi(_oauthResponse.AccessToken);
            var owner = await api.Users.GetCurrentAccountAsync();

            var account = new AccountConfiguration()
            {
                Id = owner.AccountId,
                Name = owner.Name.DisplayName,
                Type = _isAccessRestricted ? StorageType.DropboxRestricted : StorageType.Dropbox,
                Secret = _oauthResponse.AccessToken,
            };

            return account;
        }

        public async Task Initialize()
        {
            _state = Guid.NewGuid().ToString("N");
            this.AuthorizationUrl = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token,
                _isAccessRestricted ? DropboxHelper.DropboxAppFolderOnlyClientId : DropboxHelper.DropboxFullAccessClientId, RedirectionUrl, _state);
        }

        public bool CanClaim(Uri uri, string documentTitle)
        {
            return uri.ToString().StartsWith(this.RedirectionUrl.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public Task<bool> Claim(Uri uri, string documentTitle)
        {
            var cs = new TaskCompletionSource<bool>();

            try
            {
                var result = DropboxOAuth2Helper.ParseTokenFragment(uri);
                if (result.State != _state)
                {
                    // The state in the response doesn't match the state in the request.
                    cs.SetResult(false);
                    return cs.Task;
                }

                _oauthResponse = result;
                cs.SetResult(true);
                return cs.Task;
            }
            catch (Exception ex)
            {
                cs.SetException(ex);
                return cs.Task;
            }
        }

        public Uri PreAuthorizationUrl { get { return new Uri("https://www.dropbox.com/logout"); }  }

        public Uri AuthorizationUrl { get; protected set; }
        //public Uri RedirectionUrl { get { return new Uri("http://localhost/auth_redirection"); } }
        public Uri RedirectionUrl { get { return new Uri("https://www.dropbox.com/1/oauth2/redirect_receiver"); } }
        public string FriendlyProviderName { get { return "Dropbox"; } }
    }
}