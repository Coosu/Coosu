using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class BeatmapCovers
    {
        [JsonProperty("cover")]
        public string Cover { get; set; }

        [JsonProperty("cover@2x")]
        public string Cover2X { get; set; }

        [JsonProperty("card")]
        public string Card { get; set; }

        [JsonProperty("card@2x")]
        public string Card2X { get; set; }

        [JsonProperty("list")]
        public string List { get; set; }

        [JsonProperty("list@2x")]
        public string List2X { get; set; }

        [JsonProperty("slimcover")]
        public string Slimcover { get; set; }

        [JsonProperty("slimcover@2x")]
        public string Slimcover2X { get; set; }
    }
}