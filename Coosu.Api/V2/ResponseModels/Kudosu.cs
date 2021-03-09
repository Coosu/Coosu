using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class Kudosu
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("available")]
        public int Available { get; set; }
    }
}