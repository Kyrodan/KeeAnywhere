using ACD = Azi.Amazon.CloudDrive;
using System;
using System.Threading.Tasks;
using System.Web;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;
using KeePassLib.Utility;

namespace KeeAnywhere.StorageProviders.AmazonDrive
{
    public class AmazonDriveStorageConfigurator : IStorageConfigurator, IOAuth2Provider, ACD.ITokenUpdateListener
    {
        private readonly ACD.IAmazonDrive _api;
        private OAuth2Token _token;

        public AmazonDriveStorageConfigurator()
        {
            _api = AmazonDriveHelper.GetApi();
            _api.OnTokenUpdate = this;
        }

        public async Task<AccountConfiguration> CreateAccount()
        {
            var isOk = OAuth2Flow.TryAuthenticate(this);

            if (!isOk) return null;

            var profile = await _api.Profile.GetProfile();

            var account = new AccountConfiguration()
            {
                Id = profile.user_id,
                Name = profile.name,
                Type = StorageType.AmazonDrive,
                Secret = _token.RefreshToken,
            };

            return account;
        }

        public async Task Initialize()
        {
            MessageService.ShowWarning("Experimental support for Amazon Drive:", "Due to unclear Amazon Policy this provider may stop working at any time. Please do not rely on this functionality!");

            var loginUri = _api.BuildLoginUrl(this.RedirectionUrl.ToString(),
                ACD.CloudDriveScopes.ReadOther | ACD.CloudDriveScopes.Write | ACD.CloudDriveScopes.Profile);

            this.AuthorizationUrl = new Uri(loginUri);
        }

        public bool CanClaim(Uri uri, string documentTitle)
        {
            return uri.ToString().StartsWith(this.RedirectionUrl.ToString(), StringComparison.OrdinalIgnoreCase);
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

        public Uri AuthorizationUrl { get; protected set; }

        public Uri RedirectionUrl { get { return new Uri("http://localhost/auth_redirection"); } }

        public string FriendlyProviderName { get { return "Amazon Drive"; } }

        public void OnTokenUpdated(string access_token, string refresh_token, DateTime expires_in)
        {
            _token = new OAuth2Token(access_token, "bearer", null, refresh_token);
        }
    }
}