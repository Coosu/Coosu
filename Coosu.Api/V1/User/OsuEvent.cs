using System;
using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace Coosu.Api.V1.User;

/// <summary>
/// Event of a user.
/// </summary>
public class OsuEvent
{
    /// <summary>
    /// Display HTML of the event.
    /// </summary>
    [JsonProperty("display_html")]
    public string DisplayHtml { get; set; }

    /// <summary>
    /// Beatmap ID.
    /// </summary>
    [JsonProperty("beatmap_id")]
    public long BeatmapId { get; set; }

    /// <summary>
    /// Beatmap-set ID.
    /// </summary>
    [JsonProperty("beatmapset_id")]
    public long BeatmapSetId { get; set; }

    /// <summary>
    /// Event date.
    /// </summary>
    [JsonProperty("date")]
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// How "epic" this event is.
    /// </summary>
    [JsonProperty("epicfactor")]
    public long EpicFactor { get; set; }
}