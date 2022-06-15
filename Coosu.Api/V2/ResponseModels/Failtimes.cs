using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Coosu.Api.V2.ResponseModels
{
    public partial class Failtimes
    {
        [JsonProperty("fail")]
        public long[] Fail { get; set; }

        [JsonProperty("exit")]
        public long[] Exit { get; set; }
    }
}