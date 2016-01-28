using System;
using System.Runtime.Serialization;

namespace KeeAnywhere.OAuth2
{
    [DataContract]
    public class OAuth2Token
    {
        public OAuth2Token()
        {
        }

        internal OAuth2Token(string accessToken, string tokenType, int? expiresIn = null, string refreshToken = null)
        {
            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentException("Invalid OAuth 2.0 response, missing access_token.");

            AccessToken = accessToken;
            ExpiresIn = expiresIn;
            TokenType = tokenType;
            RefreshToken = refreshToken;
        }

        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "expires_in")]
        public int? ExpiresIn { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}