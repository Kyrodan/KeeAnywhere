using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.OneDrive.Sdk.Authentication;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveAuthenticationProvider : IAuthenticationProvider
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly OAuthHelper _oAuthHelper;

        public AccountSession AccountSession { get; private set; }

        public OneDriveAuthenticationProvider(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _oAuthHelper = new OAuthHelper();
        }

        public async Task AuthenticateByRefreshTokenAsync(string refreshToken)
        {
            if (refreshToken == null) throw new ArgumentNullException("refreshToken");

            await AuthenticateByAccountSessionAsync(new AccountSession {RefreshToken = refreshToken});
        }

        public async Task AuthenticateByAccountSessionAsync(AccountSession accountSession)
        {
            if (accountSession == null) throw new ArgumentNullException("accountSession");

            accountSession = await this.ProcessCachedAccountSessionAsync(accountSession);

            if (accountSession == null || string.IsNullOrEmpty(accountSession.AccessToken))
            {
                throw new ServiceException(
                    new Error
                    {
                        Code = OAuthConstants.ErrorCodes.AuthenticationFailure,
                        Message = "Failed to retrieve a valid access token"
                    });
            }

            this.AccountSession = accountSession;
        }


        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var authResult = await this.ProcessCachedAccountSessionAsync(this.AccountSession).ConfigureAwait(false);

            if (authResult == null)
            {
                throw new ServiceException(
                    new Error
                    {
                        Code = OAuthConstants.ErrorCodes.AuthenticationFailure,
                        Message = "Unable to retrieve a valid account session for the user. Please call AuthenticateByRefreshTokenAsync with proper refreshtoken."
                    });
            }

            if (!string.IsNullOrEmpty(authResult.AccessToken))
            {
                var tokenTypeString = string.IsNullOrEmpty(authResult.AccessTokenType)
                    ? OAuthConstants.Headers.Bearer
                    : authResult.AccessTokenType;
                request.Headers.Authorization = new AuthenticationHeaderValue(tokenTypeString, authResult.AccessToken);
            }
        }

        internal async Task<AccountSession> ProcessCachedAccountSessionAsync(AccountSession accountSession)
        {
            using (var httpProvider = new HttpProvider(ProxyTools.CreateHttpClientHandler(), true))
            {
                var processedAccountSession = await this.ProcessCachedAccountSessionAsync(accountSession, httpProvider).ConfigureAwait(false);
                return processedAccountSession;
            }
        }

        internal async Task<AccountSession> ProcessCachedAccountSessionAsync(AccountSession accountSession, IHttpProvider httpProvider)
        {
            if (accountSession != null)
            {
                var shouldRefresh = accountSession.ShouldRefresh;

                // If we don't have an access token or it's expiring see if we can refresh the access token.
                if (shouldRefresh && accountSession.CanRefresh)
                {
                    accountSession = await _oAuthHelper.RedeemRefreshTokenAsync(
                        accountSession.RefreshToken,
                        _clientId,
                        _clientSecret,
                        (string)null,
                        null,
                        //"https://login.live.com/oauth20_desktop.srf",
                        //new[] {"offline_access", "onedrive.readwrite"},
                        httpProvider).ConfigureAwait(false);

                    if (accountSession != null && !string.IsNullOrEmpty(accountSession.AccessToken))
                    {
                        return accountSession;
                    }
                }
                else if (!shouldRefresh)
                {
                    return accountSession;
                }
            }

            return null;
        }
    }
}