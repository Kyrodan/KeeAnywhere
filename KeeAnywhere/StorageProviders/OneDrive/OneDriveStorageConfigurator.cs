using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;
using KeePass.UI;
using KoenZomers.OneDrive.Api;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveStorageConfigurator : IStorageConfigurator, IOAuth2Provider
    {
        private OneDriveConsumerApi _api;
        private string _token;

        public async Task<AccountConfiguration> CreateAccount()
        {
            _api = new OneDriveConsumerApi(OneDriveHelper.OneDriveClientId, OneDriveHelper.OneDriveClientSecret);

            var isOk = OAuth2Flow.TryAuthenticate(this);
            if (!isOk) return null;

            var drive = await _api.GetDrive();

            var account = new AccountConfiguration
            {
                Type = StorageType.OneDrive,
                Name = drive.Owner.User.DisplayName,
                Id = drive.Id,
                Secret = _api.AccessToken.RefreshToken
            };


            return account;
        }

        public async Task Initialize()
        {
            this.PreAuthorizationUrl = _api.GetSignOutUri();
            this.AuthorizationUrl = _api.GetAuthenticationUri("wl.offline_access wl.skydrive_update");
        }

        public async Task<bool> Claim(Uri uri, string documentTitle)
        {
            _token = _api.GetAuthorizationTokenFromUrl(uri.ToString());
            return (_token != null);
        }

        public Uri AuthorizationUrl { get; protected set; }
        public Uri PreAuthorizationUrl { get; protected set; }
        public Uri RedirectionUrl { get {return new Uri("https://login.live.com/oauth20_desktop.srf");} }
        public string FriendlyProviderName { get { return "OneDrive"; } }
    }
}