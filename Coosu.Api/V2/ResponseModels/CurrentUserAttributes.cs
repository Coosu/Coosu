using System.Text.Json.Serialization;

namespace Coosu.Api.V2.ResponseModels;

public class CurrentUserAttributes
{
    [JsonPropertyName("pin")]
    public object? Pin { get; set; }
}