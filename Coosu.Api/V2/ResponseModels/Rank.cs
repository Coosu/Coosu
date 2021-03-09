using Newtonsoft.Json;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class Rank
    {
        [JsonProperty("country")]
        public int? Country { get; set; }
    }
}