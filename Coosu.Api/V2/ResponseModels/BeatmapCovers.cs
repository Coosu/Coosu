using System;
using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class BeatmapCovers
    {
        [JsonProperty("cover")]
        public Uri Cover { get; set; }

        [JsonProperty("cover@2x")]
        public Uri Cover2X { get; set; }

        [JsonProperty("card")]
        public Uri Card { get; set; }

        [JsonProperty("card@2x")]
        public Uri Card2X { get; set; }

        [JsonProperty("list")]
        public Uri List { get; set; }

        [JsonProperty("list@2x")]
        public Uri List2X { get; set; }

        [JsonProperty("slimcover")]
        public Uri Slimcover { get; set; }

        [JsonProperty("slimcover@2x")]
        public Uri Slimcover2X { get; set; }
    }
}