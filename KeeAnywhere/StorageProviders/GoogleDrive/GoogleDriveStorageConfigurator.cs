using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;

namespace KeeAnywhere.StorageProviders.GoogleDrive
{
    public class GoogleDriveStorageConfigurator : IStorageConfigurator
    {
        public async Task<AccountConfiguration> CreateAccount()
        {
            var account = new AccountConfiguration()
            {
                Type = StorageType.GoogleDrive
            };

            var api = await GoogleDriveHelper.GetClient(account);

            var query = api.About.Get();
            query.Fields = "user";
            var about = await query.ExecuteAsync();

            account.Id = about.User.PermissionId;
            account.Name = about.User.DisplayName;
          
            return account;
        }
    }

    public class GoogleDriveStorageConfigurator_old : IStorageConfigurator, IOAuth2Provider
    {
        private GoogleAuthorizationCodeFlow _flow;
        private TokenResponse _token;

        public Task<AccountConfiguration> CreateAccount()
        {
            return TaskEx.Run(() =>
            {

                var isOk = OAuth2Flow.TryAuthenticate(this);

                if (!isOk) return null;


                return (AccountConfiguration)null;
            });
        }

        public Task Initialize()
        {
            return Task.Run(() =>
            {
                var initializer = new GoogleAuthorizationCodeFlow.Initializer();
                initializer.ClientSecrets = new ClientSecrets
                {
                    ClientId = GoogleDriveHelper.GoogleDriveClientId,
                    ClientSecret = GoogleDriveHelper.GoogleDriveClientSecret,
                };
                initializer.Scopes = new[] {DriveService.Scope.Drive};
                initializer.DataStore = null;

                _flow = new GoogleAuthorizationCodeFlow(initializer);
                var authUrl = _flow.CreateAuthorizationCodeRequest(GoogleDriveHelper.RedirectUri);
                this.AuthorizationUrl = authUrl.Build();
                this.RedirectionUrl = new Uri(GoogleAuthConsts.ApprovalUrl);
            });
        }

        public async Task<bool> Claim(Uri uri, string documentTitle)
        {
            var parts = documentTitle.Split(' ');

            if (parts.Length < 1 || parts[0] != "Success")
                return false;

            var code = parts[1].Split('=')[1];

            try
            {
                _token =
                    await
                        _flow.ExchangeCodeForTokenAsync("user", code, GoogleDriveHelper.RedirectUri,
                            CancellationToken.None);
                return _token != null;
            }
            catch (TokenResponseException)
            {
                return false;
            }
        }

        public Uri AuthorizationUrl { get; protected set; }
        public Uri RedirectionUrl { get; protected set; }
        public string FriendlyProviderName { get { return "Google Drive"; } }
    }
}