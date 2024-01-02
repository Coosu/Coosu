using System;
using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Coosu.Api.V2.ResponseModels;

public class BeatmapsetSearchResult
{
    [JsonProperty("beatmapsets")]
    public BeatmapsetSearch[] Beatmapsets { get; set; }
    [JsonProperty("cursor_string")]
    public string? CursorString { get; set; }
    [JsonProperty("search")]
    public SearchResultObject? Search { get; set; }
    [JsonProperty("recommended_difficulty")]
    public object? RecommendedDifficulty { get; set; }
    [JsonProperty("error")]
    public object? Error { get; set; }
    [JsonProperty("total")]
    public int Total { get; set; }
}

public class SearchResultObject
{
    [JsonProperty("sort")]
    public string? Sort { get; set; }
}