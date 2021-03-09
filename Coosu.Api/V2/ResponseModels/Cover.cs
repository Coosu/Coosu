using System;
using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class Cover
    {
        [JsonProperty("custom_url")]
        public Uri CustomUrl { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("id")]
        public int? Id { get; set; } // nullable for custom
    }

    public class BeatmapsetCompact
    {
        public string artist { get; set; }
        public string artist_unicode { get; set; }
        public Covers covers { get; set; }
        public string creator { get; set; }
        public int favourite_count { get; set; }
        public int id { get; set; }
        public int play_count { get; set; }
        public string preview_url { get; set; }
        public string source { get; set; }
        public string status { get; set; }
        public string title { get; set; }
        public string title_unicode { get; set; }
        public long user_id { get; set; }
        public string video { get; set; }

    }

    public class Covers
    {
        public string cover { get; set; }
        [JsonProperty("cover@2x")]
        public string cover2x { get; set; }
        public string card { get; set; }
        [JsonProperty("card@2x")]
        public string card2x { get; set; }
        public string list { get; set; }
        [JsonProperty("list@2x")]
        public string list2x { get; set; }
        public string slimcover { get; set; }
        [JsonProperty("slimcover@2x")]
        public string slimcover2x { get; set; }

    }
}