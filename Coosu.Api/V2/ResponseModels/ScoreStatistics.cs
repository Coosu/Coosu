using System.Text.Json.Serialization;

namespace Coosu.Api.V2.ResponseModels;

public class ScoreStatistics
{
    [JsonPropertyName("count_50")]
    public long Count50 { get; set; }

    [JsonPropertyName("count_100")]
    public long Count100 { get; set; }

    [JsonPropertyName("count_300")]
    public long Count300 { get; set; }

    [JsonPropertyName("count_geki")]
    public long CountGeki { get; set; }

    [JsonPropertyName("count_katu")]
    public long CountKatu { get; set; }

    [JsonPropertyName("count_miss")]
    public long CountMiss { get; set; }
}