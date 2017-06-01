using System;
using System.Threading.Tasks;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;

namespace KeeAnywhere.StorageProviders.HubiC
{
    public class HubiCStorageConfigurator : IStorageConfigurator , IOAuth2Provider
    {
        private OAuth2Token _token;

        public async Task<AccountConfiguration> CreateAccount()
        {
            var isOk = OAuth2Flow.TryAuthenticate(this);

            if (!isOk) return null;

            var hubicAccount = await HubiCHelper.GetAccountAsync(_token.AccessToken);

            var account = new AccountConfiguration()
            {
                Type = StorageType.HubiC,
                Id = hubicAccount.EMail,
                Name = string.Format("{0} {1}", hubicAccount.FirstName, hubicAccount.LastName),
                Secret = _token.RefreshToken,
            };
            
            return account;
        }

        public async Task Initialize()
        {
            this.AuthorizationUrl = HubiCHelper.GetAuthorizationUri();
            this.RedirectionUrl = new Uri(HubiCHelper.RedirectUri);
        }

        public bool CanClaim(Uri uri, string documentTitle)
        {
            return uri.ToString().StartsWith(this.RedirectionUrl.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> Claim(Uri uri, string documentTitle)
        {
            //_token = HubiCHelper.GetAccessTokenFromFragment(uri);

            string code = null;

            foreach (var pair in uri.Query.TrimStart('?').Split('&'))
            {
                var elements = pair.Split('=');
                if (elements.Length != 2)
                {
                    continue;
                }

                switch (elements[0])
                {
                    case "code":
                        code = Uri.UnescapeDataString(elements[1]);
                        break;
                }
            }

            if (code == null) return false;

            _token = await HubiCHelper.ProcessCodeFlowAsync(code);

            return _token != null;
        }

        public Uri PreAuthorizationUrl { get { return null; } }
        public Uri AuthorizationUrl { get; protected set; }
        public Uri RedirectionUrl { get; protected set; }
        public string FriendlyProviderName { get { return "hubiC"; } }
    }
}