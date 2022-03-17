using System;
using System.Text.Json.Serialization;

namespace Coosu.Api.V2.ResponseModels;

public class Score
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("user_id")]
    public long UserId { get; set; }

    [JsonPropertyName("accuracy")]
    public double Accuracy { get; set; }

    [JsonPropertyName("mods")]
    public string[] Mods { get; set; } = null!;

    [JsonPropertyName("score")]
    public long ScoreScore { get; set; }

    [JsonPropertyName("max_combo")]
    public long MaxCombo { get; set; }

    [JsonPropertyName("passed")]
    public bool Passed { get; set; }

    [JsonPropertyName("perfect")]
    public bool Perfect { get; set; }

    [JsonPropertyName("statistics")]
    public ScoreStatistics Statistics { get; set; } = null!;

    [JsonPropertyName("rank")]
    public string Rank { get; set; } = null!;

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("best_id")]
    public long? BestId { get; set; }

    [JsonPropertyName("pp")]
    public double? PP { get; set; }

    [JsonPropertyName("mode")]
    public string ModeString { get; set; } = null!;

    [JsonPropertyName("mode_int")]
    public GameMode Mode { get; set; }

    [JsonPropertyName("replay")]
    public bool Replay { get; set; }

    [JsonPropertyName("current_user_attributes")]
    public CurrentUserAttributes CurrentUserAttributes { get; set; } = null!;

    [JsonPropertyName("beatmap")]
    public Beatmap Beatmap { get; set; } = null!;

    [JsonPropertyName("beatmapset")]
    public Beatmapset Beatmapset { get; set; } = null!;

    [JsonPropertyName("user")]
    public User User { get; set; } = null!;

    [JsonPropertyName("weight")]
    public ScoreWeight? Weight { get; set; }
}