using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Coosu.Api.V2.ResponseModels;

public partial class Page
{
    [JsonProperty("html")]
    public string Html { get; set; }

    [JsonProperty("raw")]
    public string Raw { get; set; }
}