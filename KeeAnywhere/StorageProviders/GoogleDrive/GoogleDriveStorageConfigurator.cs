using System;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;

namespace KeeAnywhere.StorageProviders.GoogleDrive
{
    public class GoogleDriveStorageConfigurator : IStorageConfigurator, IOAuth2Provider
    {
        private string _state;

        public async Task<AccountConfiguration> CreateAccount()
        {
            var isOk = OAuth2Flow.TryAuthenticate(this);

            if (!isOk) return null;

            return null;
        }

        public void Initialize()
        {
            _state = Guid.NewGuid().ToString("N");
            var url = string.Format(GoogleDriveHelper.AuthUrl, GoogleDriveHelper.GoogleDriveClientId, "urn:ietf:wg:oauth:2.0:oob:auto", _state, Uri.EscapeUriString("https://www.googleapis.com/auth/drive"));

            this.AuthorizationUrl = new Uri(url);
        }

        public bool Claim(Uri uri, string documentTitle)
        {
            throw new NotImplementedException();
        }

        public Uri AuthorizationUrl { get; protected set; }
        public Uri RedirectionUrl { get { return new Uri("https://accounts.google.com/o/oauth2/approval"); } }
    }
}