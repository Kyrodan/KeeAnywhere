using ACD = Azi.Amazon.CloudDrive;
using System;
using System.Threading.Tasks;
using System.Web;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;

namespace KeeAnywhere.StorageProviders.AmazonDrive
{
    public class AmazonDriveStorageConfigurator : IStorageConfigurator, IOAuth2Provider, ACD.ITokenUpdateListener
    {
        private readonly ACD.IAmazonDrive _api;
        private OAuth2Token _token;

        public AmazonDriveStorageConfigurator()
        {
            _api = new ACD.AmazonDrive(AmazonDriveHelper.AmazonDriveClientId, AmazonDriveHelper.AmazonDriveClientSecret);
            _api.OnTokenUpdate = this;
        }

        public async Task<AccountConfiguration> CreateAccount()
        {
            var isOk = OAuth2Flow.TryAuthenticate(this);

            if (!isOk) return null;

            var amazonAccount = _api.Account;

            var account = new AccountConfiguration()
            {
                Id = "amzn",
                Name = "Amazon Account",
                Type = StorageType.AmazonDrive,
                Secret = _token.RefreshToken,
            };

            return account;
        }

        public async Task Initialize()
        {
        }

        public async Task<bool> Claim(Uri uri, string documentTitle)
        {
            var error = HttpUtility.ParseQueryString(uri.Query).Get("error_description");

            if (error != null)
            {
                throw new InvalidOperationException(error);
            }

            var code = HttpUtility.ParseQueryString(uri.Query).Get("code");

            var isOk = await _api.AuthenticationByCode(code, this.RedirectionUrl.ToString());

            return isOk;
        }

        public Uri PreAuthorizationUrl { get { return null; }  }

        public Uri AuthorizationUrl {
            get { return new Uri(_api.BuildLoginUrl(this.RedirectionUrl.ToString(), ACD.CloudDriveScopes.ReadOther | ACD.CloudDriveScopes.Write)); }
        }

        public Uri RedirectionUrl { get { return new Uri("http://localhost/auth_redirection"); } }

        public string FriendlyProviderName { get { return "Amazon Drive"; } }

        public void OnTokenUpdated(string access_token, string refresh_token, DateTime expires_in)
        {
            _token = new OAuth2Token(access_token, "bearer", null, refresh_token);
        }
    }
}