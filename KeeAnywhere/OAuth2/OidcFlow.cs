using IdentityModel;
using IdentityModel.OidcClient;
using KeeAnywhere.Configuration;
using KeeAnywhere.StorageProviders;
using KeePassLib.Utility;
using System;
using System.Linq;
using System.Text;
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

        public OidcFlow(StorageType type, string authority, string[] scopes, string clientId, string clientSecret)
        {
            this.m_type = type;
            this.m_authority = authority;
            this.m_scopes = scopes;
            this.m_clientId = clientId;
            this.m_clientSecret = clientSecret;

        }

        async public Task<AccountConfiguration> AuthorizeAsync()
        {
            var f = new OidcWaitForm();
            f.InitEx(this.m_type);
            f.Show();

            var browser = new OidcSystemBrowser();

            var allScopes = new[]            {
                OidcConstants.StandardScopes.OpenId,
                OidcConstants.StandardScopes.Profile
            }.Concat(this.m_scopes);

            var scopes = String.Join(" ", allScopes);

            var options = new OidcClientOptions
            {
                Authority = this.m_authority, ///.well-known/openid-configuration
                ClientId = this.m_clientId,
                ClientSecret = this.m_clientSecret,
                RedirectUri = browser.RedirectUri,
                Scope = scopes,
                Browser = browser,
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.FormPost
            };

            //var serilog = new LoggerConfiguration()
            //    .MinimumLevel.Verbose()
            //    .Enrich.FromLogContext()
            //    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}")
            //    .CreateLogger();

            //options.LoggerFactory.AddSerilog(serilog);

            //options.Policy.RequireAuthorizationCodeHash = false;
            //options.Policy.RequireIdentityTokenSignature = false;
            options.Policy.Discovery.ValidateEndpoints = false;


            var client = new OidcClient(options);


            LoginResult credential = null;
            try
            {
                credential = await client.LoginAsync(null, f.CancellationToken);
            }
            catch (OperationCanceledException)
            {
                credential = null;
            }

            f.Close();

            if (credential == null || credential.IsError)
            {
                MessageService.ShowWarning("Authorization failed:", credential.Error);
                return null;
            }

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
    }
}
