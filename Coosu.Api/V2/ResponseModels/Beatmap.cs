using System;
using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Coosu.Api.V2.ResponseModels;

public partial class Beatmap : IBeatmap
{
    [JsonProperty("beatmapset_id")]
    public long BeatmapsetId { get; set; }

    [JsonProperty("difficulty_rating")]
    public double DifficultyRating { get; set; }

    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("mode")]
    public string Mode { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("total_length")]
    public long TotalLength { get; set; }

    [JsonProperty("user_id")]
    public long UserId { get; set; }

    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("accuracy")]
    public double Od { get; set; }

    [JsonProperty("ar")]
    public double Ar { get; set; }

    [JsonProperty("bpm")]
    public double? Bpm { get; set; }

    [JsonProperty("convert")]
    public bool Convert { get; set; }

    [JsonProperty("count_circles")]
    public long CountCircles { get; set; }

    [JsonProperty("count_sliders")]
    public long CountSliders { get; set; }

    [JsonProperty("count_spinners")]
    public long CountSpinners { get; set; }

    [JsonProperty("cs")]
    public double Cs { get; set; }

    [JsonProperty("deleted_at")]
    public object DeletedAt { get; set; }

    [JsonProperty("drain")]
    public double Hp { get; set; }

    [JsonProperty("hit_length")]
    public long HitLength { get; set; }

    [JsonProperty("is_scoreable")]
    public bool IsScoreable { get; set; }

    [JsonProperty("last_updated")]
    public DateTimeOffset LastUpdated { get; set; }

    [JsonProperty("mode_int")]
    public long ModeInt { get; set; }

    [JsonProperty("passcount")]
    public long Passcount { get; set; }

    [JsonProperty("playcount")]
    public long Playcount { get; set; }

    [JsonProperty("ranked")]
    public long Ranked { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("checksum")]
    public string Checksum { get; set; }

    [JsonProperty("failtimes")]
    public Failtimes Failtimes { get; set; }

    [JsonProperty("max_combo")]
    public long? MaxCombo { get; set; }
}