using System;
using System.Threading.Tasks;
using Dropbox.Api;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;

namespace KeeAnywhere.StorageProviders.Dropbox
{
    public class DropboxStorageConfigurator : IStorageConfigurator, IOAuth2Provider
    {
        private string _state;
        private OAuth2Response _oauthResponse;

        public async Task<AccountConfiguration> CreateAccount()
        {
            var isOk = OAuth2Flow.TryAuthenticate(this);

            if (!isOk) return null;

            var api = new DropboxClient(_oauthResponse.AccessToken);

            var owner = await api.Users.GetCurrentAccountAsync();

            var account = new AccountConfiguration()
            {
                Id = owner.AccountId,
                Name = owner.Email,
                Type = StorageType.Dropbox,
                Secret = _oauthResponse.AccessToken,
            };

            return account;
        }

        public Task Initialize()
        {
            return TaskEx.Run(() =>
            {
                _state = Guid.NewGuid().ToString("N");
                this.AuthorizationUrl = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token,
                    DropboxHelper.DropboxClientId, RedirectionUrl, _state);
            });
        }

        public Task<bool> Claim(Uri uri, string documentTitle)
        {
            return TaskEx.Run(() =>
            {
                try
                {
                    var result = DropboxOAuth2Helper.ParseTokenFragment(uri);
                    if (result.State != _state)
                    {
                        // The state in the response doesn't match the state in the request.
                        return false;
                    }

                    _oauthResponse = result;
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        public Uri AuthorizationUrl { get; private set; }
        //public Uri RedirectionUrl { get { return new Uri("http://localhost/auth_redirection"); } }
        public Uri RedirectionUrl { get { return new Uri("https://www.dropbox.com/1/oauth2/redirect_receiver"); } }
        public string FriendlyProviderName { get { return "Dropbox"; } }
    }
}