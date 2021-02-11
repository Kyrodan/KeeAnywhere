using IdentityModel;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using KeeAnywhere.Configuration;
using KeeAnywhere.StorageProviders;
using KeePassLib.Utility;
using System;
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
        private string[] m_scopes;
        private string m_clientId;
        private string m_clientSecret;
        private int m_startPort;
        private int m_endPort;

        public OidcFlow(StorageType type, string clientId, string clientSecret, string[] scopes, int startPort = 0, int endPort = 0)
        {
            this.m_type = type;
            this.m_scopes = scopes;
            this.m_clientId = clientId;
            this.m_clientSecret = clientSecret;
            this.m_startPort = startPort;
            this.m_endPort = endPort;
        }

        async public Task<AccountConfiguration> AuthorizeOidAsync(string authority)
        {
            var f = CreateWaitForm();
            f.Show();

            var browser = CreateBrowser();

            var allScopes = new[]            {
                OidcConstants.StandardScopes.OpenId,
                OidcConstants.StandardScopes.Profile
            }.Concat(this.m_scopes);

            var scopes = String.Join(" ", allScopes);

            var options = new OidcClientOptions
            {
                Authority = authority, ///.well-known/openid-configuration
                ClientId = this.m_clientId,
                ClientSecret = this.m_clientSecret,
                RedirectUri = browser.RedirectUri,
                Scope = scopes,
                Browser = browser,
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.FormPost
            };

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

        private OidcSystemBrowser CreateBrowser()
        {
            return new OidcSystemBrowser(m_startPort, m_endPort);
        }

        private OidcWaitForm CreateWaitForm()
        {
            var f = new OidcWaitForm();
            f.InitEx(this.m_type);

            return f;
        }

        async public Task<AccountConfiguration> AuthorizeOauth2Async(string authorizeEndpoint, string tokenEndpoint)
        {
            var f = CreateWaitForm();
            f.Show();

            var browser = CreateBrowser();

            var pkce = CreatePkceData();

            var state = new AuthorizeState()
            {
                RedirectUri = browser.RedirectUri,
                State = CryptoRandom.CreateUniqueId(16), //Guid.NewGuid().ToString("N"),
                CodeVerifier = pkce.CodeVerifier
            };

            var scopes = String.Join(" ", m_scopes);

            //var startUrl = getStartUrl(browser.RedirectUri);
            state.StartUrl = new RequestUrl(authorizeEndpoint).CreateAuthorizeUrl(m_clientId, OidcConstants.ResponseTypes.Code, scopes, state.RedirectUri, state.State, null, null, null, null, null, pkce.CodeChallenge, OidcConstants.CodeChallengeMethods.Sha256);

            var options = new BrowserOptions(state.StartUrl, state.RedirectUri)
            {
                Timeout = TimeSpan.FromSeconds(300),
                DisplayMode = DisplayMode.Visible,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect
            };

            var browserResult = await browser.InvokeAsync(options, f.CancellationToken);
            if (browserResult.ResultType != BrowserResultType.Success)
            {
                return null;
            }

            //var result = await ProcessOAuth2ResponseAsync(tokenEndpoint, browserResult.Response, state, f.CancellationToken);
            var authorizeResponse = new AuthorizeResponse(browserResult.Response);
            if (authorizeResponse.IsError)
            {
                //return new LoginResult(authorizeResponse.ErrorDescription);
                return null;
            }

            if (string.IsNullOrEmpty(authorizeResponse.Code))
            {
                //return new LoginResult("Missing authorization code.");
                return null;
            }

            if (string.IsNullOrEmpty(authorizeResponse.State))
            {
                //return new LoginResult("Missing state.");
                return null;
            }

            if (!string.Equals(state.State, authorizeResponse.State, StringComparison.Ordinal))
            {
                //return new LoginResult("Invalid state.");
                return null;
            }

            var tokenResponse = await RedeemOauth2CodeAsync(tokenEndpoint, authorizeResponse.Code, state, f.CancellationToken);
            if (tokenResponse.IsError)
            {
                //return new LoginResult(tokenResponse.ErrorDescription);
                return null;
            }


            //if (tokenResponse.)
            //var tokenResponseValidationResult = await ValidateTokenResponseAsync(tokenResponse, state, requireIdentityToken: false, cancellationToken: cancellationToken);
            //if (tokenResponseValidationResult.IsError)
            //{
            //    return new LoginResult(tokenResponseValidationResult.ErrorDescription);
            //}


            //, Func<string, Task<AccountConfiguration>> performSecondHalfAsync
            //var account = await performSecondHalfAsync(result.Response);

            f.Close();

            var account = new AccountConfiguration()
            {
                Secret = tokenResponse.RefreshToken
            };
            return account;
        }

        //private async Task<LoginResult> ProcessOAuth2ResponseAsync(string tokenEndpoint, string response, AuthorizeState state, CancellationToken cancellationToken)
        //{
        //    var authorizeResponse = new AuthorizeResponse(response);
        //    if (authorizeResponse.IsError)
        //    {
        //        return new LoginResult(authorizeResponse.ErrorDescription);
        //    }

        //    if (string.IsNullOrEmpty(authorizeResponse.Code))
        //    {
        //        return new LoginResult("Missing authorization code.");
        //    }

        //    if (string.IsNullOrEmpty(authorizeResponse.State))
        //    {
        //        return new LoginResult("Missing state.");
        //    }

        //    if (!string.Equals(state.State, authorizeResponse.State, StringComparison.Ordinal))
        //    {
        //        return new LoginResult("Invalid state.");
        //    }

        //    var tokenResponse = await RedeemOauth2CodeAsync(tokenEndpoint, authorizeResponse.Code, state, cancellationToken);
        //    if (tokenResponse.IsError)
        //    {
        //        return new LoginResult(tokenResponse.ErrorDescription);
        //    }


        //    //if (tokenResponse.)
        //    //var tokenResponseValidationResult = await ValidateTokenResponseAsync(tokenResponse, state, requireIdentityToken: false, cancellationToken: cancellationToken);
        //    //if (tokenResponseValidationResult.IsError)
        //    //{
        //    //    return new LoginResult(tokenResponseValidationResult.ErrorDescription);
        //    //}


        //    return new LoginResult()
        //    {
        //        AccessToken = tokenResponse.AccessToken,
        //        RefreshToken = tokenResponse.RefreshToken,
        //    };
        //}

        private async Task<TokenResponse> RedeemOauth2CodeAsync(string tokenEndpoint, string code, AuthorizeState state, CancellationToken cancellationToken)
        {

            //var client = _options.CreateClient();
            var client = new HttpClient();
            var tokenResult = await client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = tokenEndpoint,

                ClientId = m_clientId,
                ClientSecret = m_clientSecret,
                //ClientAssertion = _options.ClientAssertion,
                //ClientCredentialStyle = _options.TokenClientCredentialStyle,

                Code = code,
                RedirectUri = state.RedirectUri,
                CodeVerifier = state.CodeVerifier,
                //Parameters = backChannelParameters.Extra ?? new Parameters()
            }, cancellationToken).ConfigureAwait(false);

            return tokenResult;
        }

        //internal async Task<TokenResponseValidationResult> ValidateTokenResponseAsync(TokenResponse response, AuthorizeState state, bool requireIdentityToken, CancellationToken cancellationToken = default)
        //{
        //    // token response must contain an access token
        //    if (response.AccessToken.IsMissing())
        //    {
        //        return new TokenResponseValidationResult("Access token is missing on token response.");
        //    }

        //    if (requireIdentityToken)
        //    {
        //        // token response must contain an identity token (openid scope is mandatory)
        //        if (response.IdentityToken.IsMissing())
        //        {
        //            return new TokenResponseValidationResult("Identity token is missing on token response.");
        //        }
        //    }

        //    if (response.IdentityToken.IsPresent())
        //    {
        //        IIdentityTokenValidator validator;
        //        if (_options.IdentityTokenValidator == null)
        //        {
        //            if (_options.Policy.RequireIdentityTokenSignature == false)
        //            {
        //                validator = new NoValidationIdentityTokenValidator();
        //            }
        //            else
        //            {
        //                throw new InvalidOperationException("No IIdentityTokenValidator is configured. Either explicitly set a validator on the options, or set RequireIdentityTokenSignature to false to skip validation.");
        //            }
        //        }
        //        else
        //        {
        //            validator = _options.IdentityTokenValidator;
        //        }

        //        var validationResult = await validator.ValidateAsync(response.IdentityToken, _options, cancellationToken);

        //        if (validationResult.Error == "invalid_signature")
        //        {
        //            await _refreshKeysAsync(cancellationToken);
        //            validationResult = await _options.IdentityTokenValidator.ValidateAsync(response.IdentityToken, _options, cancellationToken);
        //        }

        //        if (validationResult.IsError)
        //        {
        //            return new TokenResponseValidationResult(validationResult.Error ?? "Identity token validation error");
        //        }

        //        // validate nonce
        //        if (state != null)
        //        {
        //            if (!ValidateNonce(state.Nonce, validationResult.User))
        //            {
        //                return new TokenResponseValidationResult("Invalid nonce.");
        //            }
        //        }

        //        // validate at_hash
        //        if (!string.Equals(validationResult.SignatureAlgorithm, "none", StringComparison.OrdinalIgnoreCase))
        //        {
        //            var atHash = validationResult.User.FindFirst(JwtClaimTypes.AccessTokenHash);
        //            if (atHash == null)
        //            {
        //                if (_options.Policy.RequireAccessTokenHash)
        //                {
        //                    return new TokenResponseValidationResult("at_hash is missing.");
        //                }
        //            }
        //            else
        //            {
        //                if (!_crypto.ValidateHash(response.AccessToken, atHash.Value, validationResult.SignatureAlgorithm))
        //                {
        //                    return new TokenResponseValidationResult("Invalid access token hash.");
        //                }
        //            }
        //        }

        //        return new TokenResponseValidationResult(validationResult);
        //    }

        //    return new TokenResponseValidationResult((IdentityTokenValidationResult)null);
        //}

        private Pkce CreatePkceData()
        {
            var pkce = new Pkce
            {
                CodeVerifier = CryptoRandom.CreateUniqueId()
            };

            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pkce.CodeVerifier));
                pkce.CodeChallenge = Base64Url.Encode(challengeBytes);
            }

            return pkce;
        }

        private class Pkce
        {
            public string CodeVerifier { get; set; }
            public string CodeChallenge { get; set; }
        }
    }
}
