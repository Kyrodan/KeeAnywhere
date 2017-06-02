using System;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;
using Kyrodan.HiDrive;
using Kyrodan.HiDrive.Authentication;

namespace KeeAnywhere.StorageProviders.HiDrive
{
    public class HiDriveStorageConfigurator : IStorageConfigurator, IOAuth2Provider
    {
        private readonly IHiDriveAuthenticator _authenticator;

        public HiDriveStorageConfigurator()
        {
            _authenticator = HiDriveHelper.GetAuthenticator();
        }

        public async Task<AccountConfiguration> CreateAccount()
        {
            var isOk = OAuth2Flow.TryAuthenticate(this);

            if (!isOk) return null;

            var client = HiDriveHelper.GetClient(_authenticator);
            //var fields = new[]
            //{
            //    User.Fields.Account, User.Fields.Alias, User.Fields.Description, User.Fields.Protocols, User.Fields.IsAdmin,
            //    User.Fields.EMail, User.Fields.IsEncrypted, User.Fields.Home, User.Fields.HomeId, User.Fields.IsOwner, User.Fields.Language, 
            //};
            var user = await client.User.Me.Get().ExecuteAsync();
            
            var account = new AccountConfiguration()
            {
                Type = StorageType.HiDrive,
                Id = user.Account,
                Name = user.Alias,
                Secret = _authenticator.Token.RefreshToken,
            };

            return account;
        }

        public async Task Initialize()
        {
            this.AuthorizationUrl = new Uri(_authenticator.GetAuthorizationCodeRequestUrl(new AuthorizationScope(AuthorizationRole.User, AuthorizationPermission.ReadWrite)));
            this.RedirectionUrl = new Uri(HiDriveHelper.RedirectUri);
        }

        public bool CanClaim(Uri uri, string documentTitle)
        {
            return uri.ToString().StartsWith(this.RedirectionUrl.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> Claim(Uri uri, string documentTitle)
        {
            var code = _authenticator.GetAuthorizationCodeFromResponseUrl(uri);

            if (code == null) return false;

            var token = await _authenticator.AuthenticateByAuthorizationCodeAsync(code);

            return  token != null;
        }

        public Uri PreAuthorizationUrl { get { return null; } }
        public Uri AuthorizationUrl { get; protected set; }
        public Uri RedirectionUrl { get; protected set; }
        public string FriendlyProviderName { get { return "HiDrive"; } }
    }
}