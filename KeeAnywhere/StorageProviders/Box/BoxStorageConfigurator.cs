using System;
using System.Threading.Tasks;
using Box.V2.Config;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;
using KeePassLib.Utility;

namespace KeeAnywhere.StorageProviders.Box
{
    public class BoxStorageConfigurator : IStorageConfigurator
    {
        public async Task<AccountConfiguration> CreateAccount()
        {
            var f = new OidcWaitForm();
            f.InitEx(StorageType.Box);
            f.Show();


            var clientId = BoxHelper.Config.ClientId;
            var clientSecret = BoxHelper.Config.ClientSecret;

            var browser = new OidcSystemBrowser();

            var redirectUri = browser.RedirectUri;

            var config = new BoxConfig(clientId, clientSecret, new Uri(redirectUri));

            var uri = config.AuthCodeUri;
            AccountConfiguration account = null;
            string error = null;

            try
            {
                var query = await browser.GetQueryStringAsync(uri.AbsoluteUri, f.CancellationToken);

                if (query == null) // Cancelled
                    return null;

                // e.g. "access_denied" if the user denies the access on Box' authorization page
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

                var api = BoxHelper.GetClient();
                var token = await api.Auth.AuthenticateAsync(code);

                if (token == null || token.RefreshToken == null || token.AccessToken == null)
                {
                    throw new Exception("Unauthorized");
                }

                var user = await api.UsersManager.GetCurrentUserInformationAsync();

                account = new AccountConfiguration
                {
                    Type = StorageType.Box,
                    Id = user.Id,
                    Name = user.Name,
                    Secret = token.RefreshToken
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