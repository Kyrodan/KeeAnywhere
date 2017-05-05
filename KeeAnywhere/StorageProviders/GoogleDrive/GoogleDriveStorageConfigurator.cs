using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;

namespace KeeAnywhere.StorageProviders.GoogleDrive
{
    public class GoogleDriveStorageConfigurator : IStorageConfigurator, IOAuth2Provider
    {
        private TokenResponse _token;

        public async Task<AccountConfiguration> CreateAccount()
        {
            var isOk = OAuth2Flow.TryAuthenticate(this);
            if (!isOk) return null;

            var api = await GoogleDriveHelper.GetClient(_token);

            var query = api.About.Get();
            query.Fields = "user";
            var about = await query.ExecuteAsync();

            var account = new AccountConfiguration()
            {
                Type = StorageType.GoogleDrive,
                Id = about.User.PermissionId,
                Name = about.User.DisplayName,
                Secret = _token.RefreshToken
            };

          
            return account;
        }

        public async Task Initialize()
        {
            var codeRequest = GoogleDriveHelper.AuthFlow.CreateAuthorizationCodeRequest(GoogleDriveHelper.RedirectUri);
            this.AuthorizationUrl = codeRequest.Build();
            this.PreAuthorizationUrl = new Uri(string.Format(GoogleDriveHelper.LogoutUri, AuthorizationUrl));

            this.RedirectionUrl = new Uri(GoogleAuthConsts.ApprovalUrl);
        }

        public async Task<bool> Claim(Uri uri, string documentTitle)
        {
            var parts = HttpUtility.ParseQueryString(uri.Query);

            if (parts.Count < 1 || parts.Get("response") == null)
                return false;

            var code = parts.Get("response");
            if (!code.Contains("code"))
                return false;

            code = code.Split('=')[1];

            try
            {
                _token =
                    await
                        GoogleDriveHelper.AuthFlow.ExchangeCodeForTokenAsync(null, code, GoogleDriveHelper.RedirectUri,
                            CancellationToken.None).ConfigureAwait(false);
                return _token != null;
            }
            catch (TokenResponseException)
            {
                return false;
            }
        }

        public Uri PreAuthorizationUrl { get; protected set; }
        public Uri AuthorizationUrl { get; protected set; }
        public Uri RedirectionUrl { get; protected set; }
        public string FriendlyProviderName { get { return "Google Drive"; } }
    }
}