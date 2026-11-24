using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.OidcClient.Results;
using KeeAnywhere.Configuration;
using KeeAnywhere.OAuth2;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveAuthenticationProvider : IAuthenticationProvider
    {
        private readonly OidcFlow _flow;
        private readonly AccountConfiguration _account;
        // Serializes concurrent Graph requests, so a token refresh runs only once.
        private readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);
        private RefreshTokenResult _token;

        public OneDriveAuthenticationProvider(OidcFlow flow, AccountConfiguration account)
        {
            if (account == null) throw new ArgumentNullException("account");

            _flow = flow;
            _account = account;
        }

        public async Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object> additionalAuthenticationContext = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _refreshLock.WaitAsync(cancellationToken);
            try
            {
                var token = _token;

                if (token == null || token.IsError || token.AccessTokenExpiration <= DateTime.UtcNow)
                {
                    token = await _flow.RefreshTokenAsync(_account.Secret);

                    if (token.IsError)
                    {
                        _token = null;
                        var detail = string.IsNullOrEmpty(token.ErrorDescription) ? token.Error : token.Error + ": " + token.ErrorDescription;
                        throw new ServiceException(detail);
                    }

                    // Microsoft rotates the refresh token on every refresh. Keep the newest one in the
                    // account, so it is persisted when KeePass closes (ConfigurationService.Save in
                    // KeeAnywhereExt.Terminate) - otherwise the stored token eventually becomes invalid.
                    if (!string.IsNullOrEmpty(token.RefreshToken) && token.RefreshToken != _account.Secret)
                    {
                        _account.Secret = token.RefreshToken;
                    }

                    _token = token;
                }

                var accessToken = token.AccessToken;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Add("Authorization", new AuthenticationHeaderValue(CoreConstants.Headers.Bearer, accessToken).ToString());
                }
            }
            finally
            {
                _refreshLock.Release();
            }
        }
    }
}
