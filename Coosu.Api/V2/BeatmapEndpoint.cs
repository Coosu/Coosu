using System.Threading.Tasks;
using Coosu.Api.HttpClient;
using Coosu.Api.V2.RequestModels;
using Coosu.Api.V2.ResponseModels;

namespace Coosu.Api.V2;

public class BeatmapEndpoint
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
    public async Task<Beatmap> GetBeatmap(int id)
    {
        string route = $"/beatmaps/{id}";
        var obj = await _httpClient.HttpGet<Beatmap>(OsuClientV2.BaseUri + route);
        return obj;
    }

    /// <summary>
    /// Get beatmapset detail
    /// <code>scope = public</code>
    /// </summary>
    /// <param name="id">s id</param>
    /// <returns></returns>
    public async Task<Beatmapset> GetBeatmapset(int id)
    {
        string route = $"/beatmapsets/{id}";
        var obj = await _httpClient.HttpGet<Beatmapset>(OsuClientV2.BaseUri + route);
        return obj;
    }

    /// <summary>
    /// Search beatmaps
    /// <code>scope = identify/public</code>
    /// </summary>
    /// <param name="query">keywords.</param>
    /// <returns></returns>
    public async Task<BeatmapsetSearchResult> SearchBeatmapset(string? query)
    {
        return await SearchBeatmapset(new SearchOptions
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
    public async Task<BeatmapsetSearchResult> SearchBeatmapset(SearchOptions? searchOptions = null)
    {
        string route = $"/beatmapsets/search/";
        var options = searchOptions?.GetQueryString();
        var actualRoute = string.IsNullOrWhiteSpace(options) ? route : route + "?" + options;
        var obj = await _httpClient.HttpGet<BeatmapsetSearchResult>(OsuClientV2.BaseUri + actualRoute);
        return obj;
    }
}