using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class GradeCounts
    {
        [JsonProperty("ss")]
        public long Ss { get; set; }

        [JsonProperty("ssh")]
        public long Ssh { get; set; }

        [JsonProperty("s")]
        public long S { get; set; }

        [JsonProperty("sh")]
        public long Sh { get; set; }

        [JsonProperty("a")]
        public long A { get; set; }
    }
}