using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Coosu.Api.V2.ResponseModels;

public partial class NominationsSummary
{
    [JsonProperty("current")]
    public long Current { get; set; }

    [JsonProperty("required")]
    public long NominationsSummaryRequired { get; set; }
}