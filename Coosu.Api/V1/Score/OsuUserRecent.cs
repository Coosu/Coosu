using System;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;
using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;


namespace Coosu.Api.V1.Score;

/// <summary>
/// User recent API model.
/// </summary>
public class OsuUserRecent : IScore
{
    /// <summary>
    /// Beatmap ID.
    /// </summary>
    [JsonProperty("beatmap_id")]
    public long BeatmapId { get; set; }

    /// <inheritdoc />
    [JsonProperty("score")]
    public long Score { get; set; }

    /// <inheritdoc />
    [JsonProperty("maxcombo")]
    public int MaxCombo { get; set; }

    /// <inheritdoc />
    [JsonProperty("count50")]
    public int Count50 { get; set; }

    /// <inheritdoc />
    [JsonProperty("count100")]
    public int Count100 { get; set; }

    /// <inheritdoc />
    [JsonProperty("count300")]
    public int Count300 { get; set; }

    /// <inheritdoc />
    [JsonProperty("countmiss")]
    public int CountMiss { get; set; }

    /// <inheritdoc />
    [JsonProperty("countkatu")]
    public int CountKatu { get; set; }

    /// <inheritdoc />
    [JsonProperty("countgeki")]
    public int CountGeki { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public bool IsPerfect => PerfectInt == 1;
    [JsonProperty("perfect")]
    private int PerfectInt { get; set; }

    /// <summary>
    /// Enabled mods of the score.
    /// </summary>
    [JsonProperty("enabled_mods")]
    public int RawMods { get; set; }

    public Mod EnabledMods => (Mod)RawMods;

    /// <inheritdoc />
    [JsonProperty("user_id")]
    public long UserId { get; set; }

    /// <summary>
    /// Score date. (UTC)
    /// </summary>
    [JsonIgnore]
    public DateTimeOffset Date => new(DateTime.Parse(DateString), TimeSpan.Zero);

    /// <summary>
    /// Score date string. (UTC)
    /// </summary>
    [JsonProperty("date")]
    public string DateString { get; set; }

    /// <inheritdoc />
    [JsonProperty("rank")]
    public string Rank { get; set; }
}