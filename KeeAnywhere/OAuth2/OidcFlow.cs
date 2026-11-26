using IdentityModel;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;
using KeeAnywhere.Configuration;
using KeeAnywhere.StorageProviders;
using KeePassLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeeAnywhere.OAuth2
{
    public class OidcFlow
    {
        private StorageType m_type;
        private string m_authority;
        private string[] m_scopes;
        private string m_clientId;
        private string m_clientSecret;

        public IEnumerable<int> Ports { get; set; }

        public Action<LoginRequest> PrepareLoginRequest { get; set; }

        // Scopes that must be granted by the user (e.g. Google lets the user uncheck single scopes on its consent page)
        public string[] RequiredScopes { get; set; }

        public string MissingScopesMessage { get; set; }

        public OidcFlow(StorageType type, string authority, string clientId, string clientSecret, string[] scopes)
        {
            this.m_type = type;
            this.m_authority = authority;
            this.m_scopes = scopes;
            this.m_clientId = clientId;
            this.m_clientSecret = clientSecret;
        }

        async public Task<AccountConfiguration> AuthorizeAsync()
        {
            var f = CreateWaitForm();
            f.Show();

            var options = CreateBaseOptions();

            var browser = CreateBrowser();
            options.RedirectUri = browser.RedirectUri;
            options.Browser = browser;

            var client = new OidcClient(options);

            LoginResult credential = null;
            try
            {
                var request = new LoginRequest();
                if (PrepareLoginRequest != null)
                {
                    PrepareLoginRequest(request);
                }

                credential = await client.LoginAsync(request, f.CancellationToken);
            }
            catch (OperationCanceledException)
            {
                credential = null;
            }

            f.Close();

            if (credential == null) // Cancelled by user
                return null;

            if (credential.IsError)
            {
                MessageService.ShowWarning("Authorization failed:", credential.Error);
                return null;
            }

            if (!AreRequiredScopesGranted(credential))
                return null;

            var userId = credential.User.FindFirst(JwtClaimTypes.Subject).Value;
            var userName = credential.User.FindFirst(JwtClaimTypes.Name).Value;
            var refreshToken = credential.RefreshToken;

            var account = new AccountConfiguration()
            {
                Type = this.m_type,
                Id = userId,
                Name = userName,
                Secret = refreshToken
            };

            return account;
        }

        private bool AreRequiredScopesGranted(LoginResult credential)
        {
            if (RequiredScopes == null || RequiredScopes.Length == 0)
                return true;

            // Only check if the provider reports the granted scopes
            var grantedScope = credential.TokenResponse != null ? credential.TokenResponse.Scope : null;
            if (string.IsNullOrWhiteSpace(grantedScope))
                return true;

            var granted = grantedScope.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var missing = RequiredScopes.Where(scope => !granted.Contains(scope)).ToArray();
            if (missing.Length == 0)
                return true;

            MessageService.ShowWarning(
                MissingScopesMessage ?? "Authorization failed: not all required permissions were granted.",
                "Missing: " + string.Join(", ", missing));
            return false;
        }

        public async Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken)
        {
            var client = new OidcClient(CreateBaseOptions());

            return await client.RefreshTokenAsync(refreshToken);
        }

        private OidcClientOptions CreateBaseOptions()
        {
            var scopes = string.Join(" ", m_scopes);

            var options = new OidcClientOptions
            {
                Authority = m_authority, ///.well-known/openid-configuration
                ClientId = m_clientId,
                ClientSecret = m_clientSecret,
                Scope = scopes,
                RefreshTokenInnerHttpHandler = ProxyTools.CreateHttpClientHandler(),
                BackchannelHandler = ProxyTools.CreateHttpClientHandler(),
                LoadProfile = false
            };

            options.Policy.Discovery.ValidateEndpoints = false;
            options.Policy.Discovery.ValidateIssuerName = false;

            return options;
        }

        private OidcSystemBrowser CreateBrowser()
        {
            return new OidcSystemBrowser(Ports);
        }

        private OidcWaitForm CreateWaitForm()
        {
            var f = new OidcWaitForm();
            f.InitEx(this.m_type);

            return f;
        }
    }
}
