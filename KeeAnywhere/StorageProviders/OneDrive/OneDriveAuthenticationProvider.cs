using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.OidcClient.Results;
using KeeAnywhere.OAuth2;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public class OneDriveAuthenticationProvider : IAuthenticationProvider
    {
        private OidcFlow _flow;
        private string _refreshToken;
        private RefreshTokenResult _token;

        public OneDriveAuthenticationProvider(OidcFlow flow, string refreshToken)
        {
            _flow = flow;
            _refreshToken = refreshToken;
        }

        public async Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object> additionalAuthenticationContext = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var token = _token;

            if (token == null || _token.IsError || _token.AccessTokenExpiration <= DateTime.Now)
            {
                token = await _flow.RefreshTokenAsync(_refreshToken);

                if (token.IsError)
                {
                    _token = null;
                    throw new ServiceException(_token.Error);
                }

                _token = token;
            }

            var accessToken = token.AccessToken;
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Add("Authorization", new AuthenticationHeaderValue(CoreConstants.Headers.Bearer, accessToken).ToString());
            }
        }
    }
}