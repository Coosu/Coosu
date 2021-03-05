using System;
using System.Collections.Generic;
using Coosu.Api.HttpClient;
using Coosu.Api.V2.ResponseModels;
using Newtonsoft.Json;

namespace Coosu.Api.V2
{
    public static class AuthorizationHelper
    {
        private static HttpClientUtility _httpClient
            = new HttpClientUtility();
        private const string TokenLink = "https://osu.ppy.sh/oauth/token";

        public static UserToken GetUserToken(int clientId, Uri registeredRedirectUri, string clientSecret, string code)
        {
            var dic = new Dictionary<string, string>()
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

        public static UserToken RefreshUserToken(int clientId, string clientSecret, string refreshToken)
        {
            var dic = new Dictionary<string, string>()
            {
                ["client_id"] = clientId.ToString(),
                ["client_secret"] = clientSecret,
                ["grant_type"] = "authorization_code",
                ["refresh_token"] = refreshToken
            };

            var result = _httpClient.HttpPostJson(TokenLink, dic);
            var deserializeObject = JsonConvert.DeserializeObject<UserToken>(result);
            deserializeObject.CreateTime = DateTimeOffset.Now;
            return deserializeObject;
        }

        public static UserToken GetPublicToken(int clientId, string clientSecret)
        {
            var dic = new Dictionary<string, string>()
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