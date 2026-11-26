using System;
using System.Threading.Tasks;
using db = Dropbox.Api;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;
using KeePassLib.Utility;
using System.Linq;

namespace KeeAnywhere.StorageProviders.Dropbox
{
    public class DropboxStorageConfigurator : IStorageConfigurator
    {
        private readonly bool _isAccessRestricted;

        public DropboxStorageConfigurator(bool isAccessRestricted)
        {
            this._isAccessRestricted = isAccessRestricted;
        }

        public async Task<AccountConfiguration> CreateAccount()
        {
            var f = new OidcWaitForm();
            f.InitEx(_isAccessRestricted ? StorageType.DropboxRestricted : StorageType.Dropbox);
            f.Show();

            var clientId = _isAccessRestricted ? DropboxHelper.DropboxAppFolderOnlyClientId : DropboxHelper.DropboxFullAccessClientId;
            var clientSecret = _isAccessRestricted ? DropboxHelper.DropboxAppFolderOnlyClientSecret : DropboxHelper.DropboxFullAccessClientSecret;

            string[] scopes =
            {
                "account_info.read",
                "files.metadata.write",
                "files.metadata.read",
                "files.content.write",
                "files.content.read"
            };

            var ports = Enumerable.Range(49306, 10)
                .Concat(Enumerable.Range(49996, 10))
                .Concat(Enumerable.Range(63900, 10));

            var browser = new OidcSystemBrowser(ports);

            var redirectUri = browser.RedirectUri;

            var state = Guid.NewGuid().ToString("N");
            var codeVerifier = db.DropboxOAuth2Helper.GeneratePKCECodeVerifier();
            var codeChallenge = db.DropboxOAuth2Helper.GeneratePKCECodeChallenge(codeVerifier);
            var uri = db.DropboxOAuth2Helper.GetAuthorizeUri(db.OAuthResponseType.Code, clientId, redirectUri, state, false, false, null, false, db.TokenAccessType.Offline, scopes, db.IncludeGrantedScopes.None, codeChallenge);

            AccountConfiguration account = null;
            string error = null;

            try
            {
                var query = await browser.GetQueryStringAsync(uri.AbsoluteUri, f.CancellationToken);

                if (query == null) // Cancelled
                    return null;

                // e.g. "access_denied" if the user clicks "Cancel" on Dropbox' authorization page (#290)
                var queryError = query["error"];
                if (!string.IsNullOrEmpty(queryError))
                {
                    if (queryError != "access_denied")
                        error = query["error_description"] ?? queryError;

                    return null;
                }

                var resultState = query["state"];

                if (state != resultState)
                {
                    throw new Exception("MiM-Attack?");
                }

                var code = query["code"];
                if (string.IsNullOrEmpty(code))
                {
                    error = "No authorization code received.";
                    return null;
                }

                var response = await db.DropboxOAuth2Helper.ProcessCodeFlowAsync(code, clientId, null, redirectUri, ProxyTools.CreateHttpClient(), codeVerifier);
                var api = DropboxHelper.GetApi(response.AccessToken);
                var owner = await api.Users.GetCurrentAccountAsync();

                account = new AccountConfiguration()
                {
                    Id = owner.AccountId,
                    Name = owner.Name.DisplayName,
                    Type = _isAccessRestricted ? StorageType.DropboxRestricted : StorageType.Dropbox,
                    Secret = response.RefreshToken,
                };
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            finally
            {
                f.Close();

                if (error != null)
                    MessageService.ShowWarning("Authorization failed:", error);
            }

            return account;
        }
    }
}