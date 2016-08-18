using System;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;
using Microsoft.Graph;
using Microsoft.OneDrive.Sdk.Authentication;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveStorageConfigurator : IStorageConfigurator, IOAuth2Provider
    {
        private OAuthHelper _oAuthHelper;
        private AccountSession _accountSession;

        public async Task<AccountConfiguration> CreateAccount()
        {
            _oAuthHelper = new OAuthHelper();

            var isOk = OAuth2Flow.TryAuthenticate(this);
            if (!isOk) return null;

            var api = await OneDriveHelper.GetApi(_accountSession);
            var drive = await api.Drive.Request().GetAsync().ConfigureAwait(false);

            var account = new AccountConfiguration
            {
                Type = StorageType.OneDrive,
                Name = drive.Owner.User.DisplayName,
                Id = drive.Owner.User.Id,
                Secret = _accountSession.RefreshToken
            };


            return account;
        }

        public async Task Initialize()
        {
            var url =_oAuthHelper.GetAuthorizationCodeRequestUrl(OneDriveHelper.OneDriveClientId, this.RedirectionUrl.ToString(), OneDriveHelper.Scopes);
            this.AuthorizationUrl = new Uri(url);
        }

        public async Task<bool> Claim(Uri uri, string documentTitle)
        {
            var authenticationResponseValues = UrlHelper.GetQueryOptions(uri);
            OAuthErrorHandler.ThrowIfError(authenticationResponseValues);

            string code;
            if (authenticationResponseValues != null && authenticationResponseValues.TryGetValue("code", out code))
            {
                using (var httpProvider = new HttpProvider())
                {
                    _accountSession =
                        await
                            _oAuthHelper.RedeemAuthorizationCodeAsync(code, OneDriveHelper.OneDriveClientId,
                                OneDriveHelper.OneDriveClientSecret, this.RedirectionUrl.ToString(), OneDriveHelper.Scopes,
                                httpProvider).ConfigureAwait(false);
                }
            }

            return (_accountSession != null);
        }

        public Uri AuthorizationUrl { get; protected set; }
        public Uri PreAuthorizationUrl { get { return new Uri(OneDriveHelper.SignOutUrl); } }
        public Uri RedirectionUrl { get {return new Uri(OneDriveHelper.RedirectionUrl);} }
        public string FriendlyProviderName { get { return "OneDrive"; } }
    }
}