using Coosu.Api.HttpClient;
using Coosu.Api.V2.ResponseModels;
using Newtonsoft.Json;

namespace Coosu.Api.V2
{
    public class UserEndpoint
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
        /// <param name="gameMode"></param>
        /// <returns></returns>
        public User GetOwnData(GameMode? gameMode)
        {
            string route = "/me/" + gameMode?.ToParamString();
            var json = _httpClient.HttpPostJson(OsuClientV2.BaseUri + route, null);

            var obj = JsonConvert.DeserializeObject<User>(json);
            return obj;
        }

        //public Beatmapset[] GetUserBeatmaps()
        //{

        //}
    }
}