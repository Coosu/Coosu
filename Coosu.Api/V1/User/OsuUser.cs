﻿using System;
using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace Coosu.Api.V1.User;

/// <summary>
/// User API model.
/// </summary>
public class OsuUser
{
    /// <summary>
    /// User ID.
    /// </summary>
    [JsonProperty("user_id")]
    public long UserId { get; set; }

    /// <summary>
    /// User name.
    /// </summary>
    [JsonProperty("username")]
    public string UserName { get; set; }

    /// <summary>
    /// Date the user joined. (UTC)
    /// </summary>
    [JsonProperty("join_date")]
    public DateTimeOffset JoinDate { get; set; }

    /// <summary>
    /// Count of Hit-300.
    /// Total amount for all ranked, approved, and loved beatmaps played.
    /// </summary>
    [JsonProperty("count300")]
    public long Count300 { get; set; }

    /// <summary>
    /// Count of Hit-100.
    /// Total amount for all ranked, approved, and loved beatmaps played.
    /// </summary>
    [JsonProperty("count100")]
    public long Count100 { get; set; }

    /// <summary>
    /// Count of Hit-50.
    /// Total amount for all ranked, approved, and loved beatmaps played.
    /// </summary>
    [JsonProperty("count50")]
    public long Count50 { get; set; }

    /// <summary>
    /// Total times of plays.
    /// Only counts ranked, approved, and loved beatmaps.
    /// </summary>
    [JsonProperty("playcount")]
    public long PlayCount { get; set; }

    /// <summary>
    /// Counts the best individual score on each ranked, approved, and loved beatmaps.
    /// </summary>
    [JsonProperty("ranked_score")]
    public long RankedScore { get; set; }

    /// <summary>
    /// Counts every score on ranked, approved, and loved beatmaps
    /// </summary>
    [JsonProperty("total_score")]
    public long TotalScore { get; set; }

    /// <summary>
    /// PP rank of the user.
    /// </summary>
    [JsonProperty("pp_rank")]
    public long PpRank { get; set; }

    /// <summary>
    /// Level of the user.
    /// </summary>
    [JsonProperty("level")]
    public string Level { get; set; }

    /// <summary>
    /// Total pp of the user.
    /// </summary>
    [JsonProperty("pp_raw")]
    public float? PpRaw { get; set; }

    /// <summary>
    /// Accuracy of the user.
    /// </summary>
    [JsonProperty("accuracy")]
    public string Accuracy { get; set; }

    /// <summary>
    /// Counts for SS ranks on maps.
    /// </summary>
    [JsonProperty("count_rank_ss")]
    public long CountRankSs { get; set; }

    /// <summary>
    /// Counts for SSH ranks on maps.
    /// </summary>
    [JsonProperty("count_rank_ssh")]
    public long CountRankSsh { get; set; }

    /// <summary>
    /// Counts for S ranks on maps.
    /// </summary>
    [JsonProperty("count_rank_s")]
    public long CountRankS { get; set; }

    /// <summary>
    /// Counts for SH ranks on maps.
    /// </summary>
    [JsonProperty("count_rank_sh")]
    public long CountRankSh { get; set; }

    /// <summary>
    /// Counts for A ranks on maps.
    /// </summary>
    [JsonProperty("count_rank_a")]
    public long CountRankA { get; set; }

    /// <summary>
    /// Country of the user.
    /// Uses the ISO3166-1 alpha-2 country code naming.
    /// For more information, see: http://en.wikipedia.org/wiki/ISO_3166-1_alpha-2/)
    /// </summary>
    [JsonProperty("country")]
    public string Country { get; set; }

    /// <summary>
    /// Total play seconds of the user.
    /// </summary>
    [JsonProperty("total_seconds_played")]
    public long TotalSecondsPlayed { get; set; }

    /// <summary>
    /// The user's rank in the country.
    /// </summary>
    [JsonProperty("pp_country_rank")]
    public long PpCountryRank { get; set; }

    /// <summary>
    /// Contains events for this user
    /// </summary>
    [JsonProperty("events")]
    public OsuEvent[] Events { get; set; }
}