using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.OidcClient.Results;
using KeeAnywhere.OAuth2;
using Microsoft.Graph;

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

        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var token = _token;

            if (token == null || _token.IsError || _token.AccessTokenExpiration <= DateTime.Now)
            {
                token = await _flow.RefreshTokenAsync(_refreshToken);

                if (token.IsError)
                {
                    _token = null;
                    throw new ServiceException(
                        new Error
                        {
                            //Code = GraphErrorCode.AuthenticationFailure,
                            Message = _token.Error
                        });
                }

                _token = token;
            }

            var accessToken = token.AccessToken;
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(CoreConstants.Headers.Bearer, accessToken);
            }
        }
    }
}