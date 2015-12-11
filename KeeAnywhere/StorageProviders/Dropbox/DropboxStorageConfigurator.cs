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
        private string _accessToken;

        public async Task<AccountConfiguration> CreateAccount()
        {
            var isOk = OAuth2Flow.TryAuthenticate(this);

            if (!isOk) return null;

            var api = new DropboxClient(_accessToken);

            var owner = await api.Users.GetCurrentAccountAsync();

            var account = new AccountConfiguration()
            {
                Id = owner.AccountId,
                Name = owner.Name.DisplayName,
                Type = StorageType.Dropbox,
                Secret = _accessToken,
            };

            return account;
        }

        public void Initialize()
        {
            _state = Guid.NewGuid().ToString("N");
            //this.AuthorizationUrl = DropboxOAuth2Helper.GetAuthorizeUri(DropboxHelper.DropboxClientId);
            this.AuthorizationUrl = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, DropboxHelper.DropboxClientId, RedirectionUrl, _state);
        }

        public bool Claim(Uri uri)
        {
            try
            {
                OAuth2Response result = DropboxOAuth2Helper.ParseTokenFragment(uri);
                if (result.State != _state)
                {
                    // The state in the response doesn't match the state in the request.
                    return false;
                }

                _accessToken = result.AccessToken;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Uri AuthorizationUrl { get; private set; }
        public Uri RedirectionUrl { get { return new Uri("https://localhost/auth_redirection"); } }
        //public Uri RedirectionUrl { get { return new Uri("https://www.dropbox.com/1/oauth2/redirect_receiver"); } }
    }
}