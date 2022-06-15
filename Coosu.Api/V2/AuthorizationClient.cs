using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coosu.Api.HttpClient;
using Coosu.Api.V2.ResponseModels;

namespace Coosu.Api.V2
{
    public class AuthorizationClient
    {
        private readonly HttpClientUtility _httpClientUtility;
        private const string TokenLink = "https://osu.ppy.sh/oauth/token";

        public AuthorizationClient(ClientOptions? options = null)
        {
            _httpClientUtility = new HttpClientUtility(options);
        }

        public async Task<UserToken> GetUserToken(int clientId, Uri registeredRedirectUri, string clientSecret, string code)
        {
            var dic = new Dictionary<string, string>
            {
                ["client_id"] = clientId.ToString(),
                ["client_secret"] = clientSecret,
                ["code"] = code,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = registeredRedirectUri.OriginalString,
            };

            var deserializeObject = await _httpClientUtility.HttpPost<UserToken>(TokenLink, dic);
            deserializeObject.CreateTime = DateTimeOffset.Now;
            return deserializeObject;
        }

        public async Task<UserToken> RefreshUserToken(int clientId, string clientSecret, string refreshToken)
        {
            var dic = new Dictionary<string, string>
            {
                ["client_id"] = clientId.ToString(),
                ["client_secret"] = clientSecret,
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            };

            var deserializeObject = await _httpClientUtility.HttpPost<UserToken>(TokenLink, dic);
            deserializeObject.CreateTime = DateTimeOffset.Now;
            return deserializeObject;
        }

        public async Task<UserToken> GetPublicToken(int clientId, string clientSecret)
        {
            var dic = new Dictionary<string, string>
            {
                ["client_id"] = clientId.ToString(),
                ["client_secret"] = clientSecret,
                ["grant_type"] = "client_credentials",
                ["scope"] = "public"
            };

            var deserializeObject = await _httpClientUtility.HttpPost<UserToken>(TokenLink, dic);
            deserializeObject.CreateTime = DateTimeOffset.Now;
            return deserializeObject;
        }
    }
}