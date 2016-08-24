using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using KeeAnywhere.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KeeAnywhere.StorageProviders.HubiC
{
    internal class HubiCHelper
    {
        /*
            The consumer key and the secret key included here are dummy keys.
            You should go to https://hubic.com/home/browser/developers/ to create your own application
            and get your own keys.

            This is done to prevent bots from scraping the keys from the source code posted on the web.

            Every now and then an accidental checkin of keys may occur, but these are all dummy applications
            created specifically for development that are deleted frequently and limited to the developer,
            never the real production keys.
        */

        //TODO: Change API keys!!!
        internal const string HubiCClientId = "dummy";
        internal const string HubiCClientSecret = "dummy";

        public const string AuthorizationUri = "https://api.hubic.com/oauth/auth";
        public const string RedirectUri = "https://github.com/kyrodan/KeeAnywhere/";
        public const string TokenUri = "https://api.hubic.com/oauth/token";
        public const string AccountUri = "https://api.hubic.com/1.0/account";
        public const string AccountCredentialsUri = "https://api.hubic.com/1.0/account/credentials";

        public static Uri GetAuthorizationUri()
        {
            var uriString =
                string.Format(
                    "{0}?client_id={1}&redirect_uri={2}&response_type=code&scope=account.r,usage.r,credentials.r",
                    AuthorizationUri, HubiCClientId, Uri.EscapeUriString(RedirectUri));

            return new Uri(uriString);
        }

        //public static async Task<OAuth2Token> GetAccessToken(string refreshToken)
        //{
        //    if (string.IsNullOrEmpty(refreshToken))
        //    {
        //        throw new ArgumentNullException("refreshToken");
        //    }

        //    var httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
        //        "Basic",
        //        Convert.ToBase64String(
        //            Encoding.ASCII.GetBytes(
        //                string.Format("{0}:{1}", HubiCClientId, HubiCClientSecret))));

        //    var uri = string.Format("{0}?refresh_token={1}&grant_type=refresh_token", TokenUri, refreshToken);

        //    try
        //    {
        //        var response = await httpClient.PostAsync(uri, null);

        //        var bytes = await response.Content.ReadAsByteArrayAsync();
        //        var raw = Encoding.UTF8.GetString(bytes);
        //        var token = JsonConvert.DeserializeObject<OAuth2Token>(raw);
        //        return token;
        //    }
        //    finally
        //    {
        //        httpClient.Dispose();
        //    }
        //}

        public static async Task<HubiCAccount> GetAccountAsync(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException("accessToken");
            }

            var httpClient = GetHttpClient(accessToken);
            try
            {
                var response = await httpClient.GetAsync(AccountUri);

                var bytes = await response.Content.ReadAsByteArrayAsync();
                var raw = Encoding.UTF8.GetString(bytes);
                var account = JsonConvert.DeserializeObject<HubiCAccount>(raw);
                return account;
            }
            finally
            {
                httpClient.Dispose();
            }
        }

        public static async Task<HubiCCredentials> GetCredentialsAsync(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException("accessToken");
            }

            var httpClient = GetHttpClient(accessToken);
            try
            {
                var response = await httpClient.GetAsync(AccountCredentialsUri);

                var bytes = await response.Content.ReadAsByteArrayAsync();
                var raw = Encoding.UTF8.GetString(bytes);
                var credentials = JsonConvert.DeserializeObject<HubiCCredentials>(raw);
                return credentials;
            }
            finally
            {
                httpClient.Dispose();
            }
        }

        private static HttpClient GetHttpClient(string accessToken)
        {
            var httpClient = ProxyTools.CreateHttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            httpClient.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));
            return httpClient;
        }

        public static async Task<OAuth2Token> ProcessCodeFlowAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException("code");
            }

            var httpClient = ProxyTools.CreateHttpClient();
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    {"code", code},
                    {"grant_type", "authorization_code"},
                    {"client_id", HubiCClientId},
                    {"client_secret", HubiCClientSecret}
                };

                if (!string.IsNullOrEmpty(RedirectUri))
                {
                    parameters["redirect_uri"] = RedirectUri;
                }

                var content = new FormUrlEncodedContent(parameters);
                var response = await httpClient.PostAsync(TokenUri, content);

                var raw = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(raw);

                return new OAuth2Token(
                    json["access_token"].ToString(),
                    json["token_type"].ToString(),
                    json["expires_in"].Value<int>(),
                    json["refresh_token"].ToString());
            }
            finally
            {
                httpClient.Dispose();
            }
        }

        public static async Task<OAuth2Token> GetAccessTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentNullException("refreshToken");
            }

            var httpClient = ProxyTools.CreateHttpClient();
            try
            {
                var parameters = new Dictionary<string, string>
                {
                    {"refresh_token", refreshToken},
                    {"grant_type", "refresh_token"},
                    {"client_id", HubiCClientId},
                    {"client_secret", HubiCClientSecret}
                };

                var content = new FormUrlEncodedContent(parameters);
                var response = await httpClient.PostAsync(TokenUri, content);

                var raw = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(raw);

                return new OAuth2Token(
                    json["access_token"].ToString(),
                    json["token_type"].ToString(),
                    json["expires_in"].Value<int>());
            }
            finally
            {
                httpClient.Dispose();
            }
        }

        public static OAuth2Token GetAccessTokenFromFragment(Uri redirectedUri)
        {
            if (redirectedUri == null)
            {
                throw new ArgumentNullException("redirectedUri");
            }

            var fragment = redirectedUri.Fragment;
            if (string.IsNullOrWhiteSpace(fragment))
            {
                throw new ArgumentException("The supplied uri doesn't contain a fragment", "redirectedUri");
            }

            fragment = fragment.TrimStart('#');

            string accessToken = null;
            string expiresIn = null;
            string scope = null;
            string uid = null;
            string state = null;
            string tokenType = null;

            foreach (var pair in fragment.Split('&'))
            {
                var elements = pair.Split('=');
                if (elements.Length != 2)
                {
                    continue;
                }

                switch (elements[0])
                {
                    case "access_token":
                        accessToken = Uri.UnescapeDataString(elements[1]);
                        break;
                    case "expires_in":
                        expiresIn = Uri.UnescapeDataString(elements[1]);
                        break;
                    case "scope":
                        scope = Uri.UnescapeDataString(elements[1]);
                        break;
                    case "state":
                        state = Uri.UnescapeDataString(elements[1]);
                        break;
                    case "token_type":
                        tokenType = Uri.UnescapeDataString(elements[1]);
                        break;
                    default:
                        throw new ArgumentException("Unexpected values in fragment", "redirectedUri");
                }
            }

            return new OAuth2Token(accessToken, tokenType, expiresIn != null ? int.Parse(expiresIn) : (int?) null);
        }
    }
}