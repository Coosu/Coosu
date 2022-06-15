using System.Text.Json.Serialization;

namespace Coosu.Api.V2.ResponseModels;

public class ScoreWeight
{
    [JsonPropertyName("percentage")]
    public double Percentage { get; set; }

    [JsonPropertyName("pp")]
    public double Pp { get; set; }
}