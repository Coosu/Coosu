using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

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