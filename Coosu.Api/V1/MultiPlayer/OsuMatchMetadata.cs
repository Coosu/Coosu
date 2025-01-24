using System;
using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace Coosu.Api.V1.MultiPlayer;

/// <summary>
/// Match information model of the match.
/// </summary>
public class OsuMatchMetadata
{
    /// <summary>
    /// Match ID.
    /// </summary>
    [JsonProperty("match_id")]
    public long MatchId { get; set; }

    /// <summary>
    /// Match name.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Match started time. (UTC)
    /// </summary>
    [JsonProperty("start_time")]
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// Match ended time. (UTC, NULL if not ended)
    /// </summary>
    [JsonProperty("end_time")]
    public DateTimeOffset? EndTime { get; set; }
}