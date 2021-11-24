using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public class BeatmapsetSearchResult
    {
        [JsonProperty("beatmapsets")]
        public BeatmapsetSearch[] Beatmapsets { get; set; }
        [JsonProperty("cursor")]
        public object? Cursor { get; set; }
        [JsonProperty("search")]
        public object Search { get; set; }
        [JsonProperty("recommended_difficulty")]
        public object? RecommendedDifficulty { get; set; }
        [JsonProperty("error")]
        public object? Error { get; set; }
        [JsonProperty("total")]
        public int Total { get; set; }
    }
}