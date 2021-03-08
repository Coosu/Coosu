using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class Level
    {
        [JsonProperty("current")]
        public long? Current { get; set; }

        [JsonProperty("progress")]
        public long? Progress { get; set; }
    }
}