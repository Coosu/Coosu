using Coosu.Api.HttpClient;
using Coosu.Api.V2.ResponseModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Coosu.Api.V2
{
    public class AuthorizationClient
    {
        private HttpClientUtility _httpClient;
        private const string TokenLink = "https://osu.ppy.sh/oauth/token";

        public AuthorizationClient(ClientOptions options = null)
        {
            options = options ?? new ClientOptions();
            _httpClient = new HttpClientUtility(options.Socks5ProxyOptions);
        }

        public UserToken GetUserToken(int clientId, Uri registeredRedirectUri, string clientSecret, string code)
        {
            var dic = new Dictionary<string, string>
            {
                ["client_id"] = clientId.ToString(),
                ["client_secret"] = clientSecret,
                ["code"] = code,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = registeredRedirectUri.OriginalString,
            };

            var result = _httpClient.HttpPostJson(TokenLink, dic);
            var deserializeObject = JsonConvert.DeserializeObject<UserToken>(result);
            deserializeObject.CreateTime = DateTimeOffset.Now;
            return deserializeObject;
        }

        public UserToken RefreshUserToken(int clientId, string clientSecret, string refreshToken)
        {
            var dic = new Dictionary<string, string>
            {
                ["client_id"] = clientId.ToString(),
                ["client_secret"] = clientSecret,
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            };

            var result = _httpClient.HttpPostJson(TokenLink, dic);
            var deserializeObject = JsonConvert.DeserializeObject<UserToken>(result);
            deserializeObject.CreateTime = DateTimeOffset.Now;
            return deserializeObject;
        }

        public UserToken GetPublicToken(int clientId, string clientSecret)
        {
            var dic = new Dictionary<string, string>
            {
                ["client_id"] = clientId.ToString(),
                ["client_secret"] = clientSecret,
                ["grant_type"] = "client_credentials",
                ["scope"] = "public"
            };

            var result = _httpClient.HttpPostJson(TokenLink, dic);
            var deserializeObject = JsonConvert.DeserializeObject<UserToken>(result);
            deserializeObject.CreateTime = DateTimeOffset.Now;
            return deserializeObject;
        }
    }
}