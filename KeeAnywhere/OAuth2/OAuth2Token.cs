using System;

namespace KeeAnywhere.OAuth2
{
    public class OAuth2Token
    {
        internal OAuth2Token(string accessToken, string tokenType, int? expiresIn = null, string refreshToken = null)
        {
            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentException("Invalid OAuth 2.0 response, missing access_token.");

            AccessToken = accessToken;
            ExpiresIn = expiresIn;
            TokenType = tokenType;
            RefreshToken = refreshToken;
        }

        public string AccessToken { get; private set; }

        public int? ExpiresIn { get; private set; }

        public string TokenType { get; private set; }

        public string RefreshToken { get; private set; }
    }
}