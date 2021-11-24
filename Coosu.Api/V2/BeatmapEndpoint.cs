using System;
using Coosu.Api.HttpClient;
using Coosu.Api.V2.RequestModels;
using Coosu.Api.V2.ResponseModels;
using Newtonsoft.Json;

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
        /// Get beatmap detail
        /// <code>scope = public</code>
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
        /// Get beatmapset detail
        /// <code>scope = public</code>
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

        /// <summary>
        /// Search beatmaps
        /// <code>scope = identify/public</code>
        /// </summary>
        /// <param name="query">keywords.</param>
        /// <returns></returns>
        public BeatmapsetSearchResult SearchBeatmapset(string? query)
        {
            return SearchBeatmapset(new SearchOptions
            {
                Query = query
            });
        }

        /// <summary>
        /// Search beatmaps
        /// <code>scope = identify/public</code>
        /// </summary>
        /// <param name="searchOptions">Search options.</param>
        /// <returns></returns>
        public BeatmapsetSearchResult SearchBeatmapset(SearchOptions? searchOptions = null)
        {
            string route = $"/beatmapsets/search/";
            var options = searchOptions?.GetQueryString();
            var actualRoute = string.IsNullOrWhiteSpace(options) ? route : route + "filters?" + options;
            var json = _httpClient.HttpGet(OsuClientV2.BaseUri + actualRoute);

            var obj = JsonConvert.DeserializeObject<BeatmapsetSearchResult>(json);
            return obj;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}