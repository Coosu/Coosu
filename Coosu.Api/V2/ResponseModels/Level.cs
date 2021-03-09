using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class Level
    {
        [JsonProperty("current")]
        public int Current { get; set; }

        [JsonProperty("progress")]
        public int Progress { get; set; }
    }
}