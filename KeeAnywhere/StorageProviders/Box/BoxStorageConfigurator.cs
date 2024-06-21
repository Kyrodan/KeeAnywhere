using System;
using System.Threading.Tasks;
using Box.V2.Config;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;

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
            var query = await browser.GetQueryStringAsync(uri.ToString(), f.CancellationToken);

            var code = query["code"];

            var api = BoxHelper.GetClient();
            var token = await api.Auth.AuthenticateAsync(code);

            if (token == null || token.RefreshToken == null || token.AccessToken == null)
            {
                throw new Exception("Unauthorized");
            }

            var user = await api.UsersManager.GetCurrentUserInformationAsync();

            f.Close();

            return new AccountConfiguration
            {
                Type = StorageType.Box,
                Id = user.Id,
                Name = user.Name,
                Secret = token.RefreshToken
            };
        }
    }
}