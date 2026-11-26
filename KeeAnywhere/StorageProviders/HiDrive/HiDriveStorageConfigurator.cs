using System;
using System.Linq;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;
using KeePassLib.Utility;
using Kyrodan.HiDrive.Authentication;

namespace KeeAnywhere.StorageProviders.HiDrive
{
    public class HiDriveStorageConfigurator : IStorageConfigurator
    {
        public async Task<AccountConfiguration> CreateAccount()
        {
            var f = new OidcWaitForm();
            f.InitEx(StorageType.HiDrive);
            f.Show();


            var browser = new OidcSystemBrowser();

            var redirectUri = browser.RedirectUri;

            var authenticator = HiDriveHelper.GetAuthenticator();
            var uri = authenticator.GetAuthorizationCodeRequestUrl(new AuthorizationScope(AuthorizationRole.User, AuthorizationPermission.ReadWrite), redirectUri);
            AccountConfiguration account = null;
            string error = null;

            try
            {
                var query = await browser.GetQueryStringAsync(uri.ToString(), f.CancellationToken);

                if (query == null) // Cancelled
                    return null;

                // e.g. "access_denied" if the user denies the access on HiDrive's authorization page
                var queryError = query["error"];
                if (!string.IsNullOrEmpty(queryError))
                {
                    if (queryError != "access_denied")
                        error = query["error_description"] ?? queryError;

                    return null;
                }

                var code = query["code"];
                if (string.IsNullOrEmpty(code))
                {
                    error = "No authorization code received.";
                    return null;
                }

                var token = await authenticator.AuthenticateByAuthorizationCodeAsync(code);

                var client = HiDriveHelper.GetClient(authenticator);
                var user = await client.User.Me.Get().ExecuteAsync();

                account = new AccountConfiguration()
                {
                    Type = StorageType.HiDrive,
                    Id = user.Account,
                    Name = user.Alias,
                    Secret = authenticator.Token.RefreshToken,
                };
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