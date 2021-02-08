using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class Kudosu
    {
        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("available")]
        public long Available { get; set; }
    }
}