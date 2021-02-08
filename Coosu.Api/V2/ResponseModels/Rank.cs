using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class Rank
    {
        [JsonProperty("global")]
        public long Global { get; set; }

        [JsonProperty("country")]
        public long Country { get; set; }
    }
}