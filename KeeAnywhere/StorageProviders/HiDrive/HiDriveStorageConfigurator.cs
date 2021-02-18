using System;
using System.Linq;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;
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
            var query = await browser.GetQueryStringAsync(uri.ToString(), f.CancellationToken);

            var code = query["code"];
            var token = await authenticator.AuthenticateByAuthorizationCodeAsync(code);

            var client = HiDriveHelper.GetClient(authenticator);
            var user = await client.User.Me.Get().ExecuteAsync();

            var account = new AccountConfiguration()
            {
                Type = StorageType.HiDrive,
                Id = user.Account,
                Name = user.Alias,
                Secret = authenticator.Token.RefreshToken,
            };


            f.Close();

            return account;
        }
    }
}