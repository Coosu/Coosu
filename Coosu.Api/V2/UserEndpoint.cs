using Coosu.Api.HttpClient;
using Coosu.Api.V2.ResponseModels;
using Newtonsoft.Json;
using System;
using HttpUtility = System.Web.HttpUtility;

namespace Coosu.Api.V2
{
    public class UserEndpoint : IDisposable
    {
        private readonly TokenBase _token;
        private readonly HttpClientUtility _httpClient;

        public UserEndpoint(TokenBase token) : this(token, new HttpClientUtility())
        {
        }

        internal UserEndpoint(TokenBase token, HttpClientUtility httpClient)
        {
            _token = token;
            _httpClient = httpClient;
            _httpClient.SetDefaultAuthorization(_token.TokenType, _token.AccessToken);
        }

        /// <summary>
        /// Similar to Get User but with authenticated user (token owner) as user id.
        /// <code>scope = identify</code>
        /// </summary>
        /// <param name="gameMode"><see cref="GameMode"/>. User default mode will be used if not specified.</param>
        /// <returns></returns>
        public User GetOwnData(GameMode? gameMode = null)
        {
            string route = "/me/" + gameMode?.ToParamString();
            var json = _httpClient.HttpGet(OsuClientV2.BaseUri + route);

            var obj = JsonConvert.DeserializeObject<User>(json);
            return obj;
        }

        /// <summary>
        /// Similar to Get User but with authenticated user (token owner) as user id.
        /// <code>scope = identify</code>
        /// </summary>
        /// <param name="user">Id of the user.</param>
        /// <param name="gameMode"><see cref="GameMode"/>. User default mode will be used if not specified.</param>
        /// <returns></returns>
        public User GetUser(string user, GameMode? gameMode = null)
        {
            string route = $"/users/{HttpUtility.UrlEncode(user)}/{gameMode?.ToParamString()}";
            var json = _httpClient.HttpGet(OsuClientV2.BaseUri + route);

            var obj = JsonConvert.DeserializeObject<User>(json);
            return obj;
        }

        public Beatmapset[] GetUserBeatmap(string user, UserBeatmapType type)
        {
            string route = $"/users/{HttpUtility.UrlEncode(user)}/beatmapsets/{type.ToParamString()}?limit=500&offset=0";
            var json = _httpClient.HttpGet(OsuClientV2.BaseUri + route);

            var obj = JsonConvert.DeserializeObject<Beatmapset[]>(json);
            return obj;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}