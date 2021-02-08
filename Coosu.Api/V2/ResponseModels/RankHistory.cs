using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class RankHistory
    {
        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("data")]
        public long[] Data { get; set; }
    }
}