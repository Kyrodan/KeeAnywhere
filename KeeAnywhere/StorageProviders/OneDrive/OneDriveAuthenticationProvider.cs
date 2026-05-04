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
        private readonly Action<AccountConfiguration> _onAccountChanged;
        // Serializes concurrent Graph requests so the refresh + rotation-persist runs once.
        private readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);
        private RefreshTokenResult _token;

        public OneDriveAuthenticationProvider(OidcFlow flow, AccountConfiguration account, Action<AccountConfiguration> onAccountChanged)
        {
            _flow = flow;
            _account = account;
            _onAccountChanged = onAccountChanged;
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

                    // Microsoft rotates the refresh token on every refresh; persist it or the stored secret eventually drifts to invalid_grant.
                    if (!string.IsNullOrEmpty(token.RefreshToken) && token.RefreshToken != _account.Secret)
                    {
                        _account.Secret = token.RefreshToken;
                        if (_onAccountChanged != null) _onAccountChanged(_account);
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
