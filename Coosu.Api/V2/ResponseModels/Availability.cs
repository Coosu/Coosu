using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Coosu.Api.V2.ResponseModels;

public partial class Availability
{
    [JsonProperty("download_disabled")]
    public bool DownloadDisabled { get; set; }

    [JsonProperty("more_information")]
    public string MoreInformation { get; set; }
}