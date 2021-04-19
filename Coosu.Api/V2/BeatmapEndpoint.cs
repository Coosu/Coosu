using Coosu.Api.HttpClient;
using Coosu.Api.V2.ResponseModels;
using Newtonsoft.Json;
using System;

namespace Coosu.Api.V2
{
    public class BeatmapEndpoint : IDisposable
    {
        private readonly TokenBase _token;
        private readonly HttpClientUtility _httpClient;

        public BeatmapEndpoint(TokenBase token) : this(token, new HttpClientUtility())
        {
        }

        internal BeatmapEndpoint(TokenBase token, HttpClientUtility httpClient)
        {
            _token = token;
            _httpClient = httpClient;
            _httpClient.SetDefaultAuthorization(_token.TokenType, _token.AccessToken);
        }

        /// <summary>
        /// Similar to Get User but with authenticated user (token owner) as user id.
        /// <code>scope = identify</code>
        /// </summary>
        /// <param name="id">b id</param>
        /// <returns></returns>
        public Beatmap GetBeatmap(int id)
        {
            string route = $"/beatmaps/{id}";
            var json = _httpClient.HttpGet(OsuClientV2.BaseUri + route);

            var obj = JsonConvert.DeserializeObject<Beatmap>(json);
            return obj;
        }

        /// <summary>
        /// Similar to Get User but with authenticated user (token owner) as user id.
        /// <code>scope = identify</code>
        /// </summary>
        /// <param name="id">s id</param>
        /// <returns></returns>
        public Beatmapset GetBeatmapset(int id)
        {
            string route = $"/beatmapsets/{id}";
            var json = _httpClient.HttpGet(OsuClientV2.BaseUri + route);

            var obj = JsonConvert.DeserializeObject<Beatmapset>(json);
            return obj;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}