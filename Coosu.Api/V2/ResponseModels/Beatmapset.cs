﻿using System;
using System.Linq;
using JsonPropertyAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using JsonConverterAttribute = System.Text.Json.Serialization.JsonConverterAttribute;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace Coosu.Api.V2.ResponseModels;

public partial class Beatmapset : IBeatmapset
{
    [JsonProperty("artist")]
    public string Artist { get; set; }

    [JsonProperty("artist_unicode")]
    public string ArtistUnicode { get; set; }

    [JsonProperty("covers")]
    public BeatmapCovers Covers { get; set; }

    [JsonProperty("creator")]
    public string Creator { get; set; }

    [JsonProperty("favourite_count")]
    public long FavouriteCount { get; set; }

    [JsonProperty("hype")]
    public object Hype { get; set; }

    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("nsfw")]
    public bool Nsfw { get; set; }

    [JsonProperty("play_count")]
    public long PlayCount { get; set; }

    [JsonProperty("preview_url")]
    public string PreviewUrl { get; set; }

    [JsonProperty("source")]
    public string Source { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("title_unicode")]
    public string TitleUnicode { get; set; }

    [JsonProperty("track_id")]
    public long? TrackId { get; set; }

    [JsonProperty("user_id")]
    public long UserId { get; set; }

    [JsonProperty("video")]
    public bool Video { get; set; }

    [JsonProperty("availability")]
    public Availability Availability { get; set; }

    [JsonProperty("bpm")]
    public double? Bpm { get; set; }

    [JsonProperty("can_be_hyped")]
    public bool CanBeHyped { get; set; }

    [JsonProperty("discussion_enabled")]
    public bool DiscussionEnabled { get; set; }

    [JsonProperty("discussion_locked")]
    public bool DiscussionLocked { get; set; }

    [JsonProperty("is_scoreable")]
    public bool IsScoreable { get; set; }

    [JsonProperty("last_updated")]
    public DateTimeOffset LastUpdated { get; set; }

    [JsonProperty("legacy_thread_url")]
    public string? LegacyThreadUrl { get; set; }

    [JsonProperty("nominations_summary")]
    public NominationsSummary NominationsSummary { get; set; }

    [JsonProperty("ranked")]
    public long Ranked { get; set; }

    [JsonProperty("ranked_date")]
    public DateTimeOffset? RankedDate { get; set; }

    [JsonProperty("storyboard")]
    public bool Storyboard { get; set; }

    [JsonProperty("submitted_date")]
    public DateTimeOffset SubmittedDate { get; set; }

    [JsonProperty("tags")]
    public string Tags { get; set; }

    [JsonProperty("beatmaps")]
    public Beatmap[] Beatmaps { get; set; }

    [JsonProperty("converts")]
    public Beatmap[] Converts { get; set; }

    [JsonProperty("description")]
    public BeatmapsetDescription Description { get; set; }

    [JsonProperty("genre")]
    public Genre Genre { get; set; }

    [JsonProperty("language")]
    public Genre Language { get; set; }

    [JsonProperty("ratings")]
    public int[] Ratings { get; set; }

    [JsonProperty("recent_favourites")]
    public UserLight[] RecentFavourites { get; set; }

    [JsonProperty("user")]
    public UserLight User { get; set; }

    IBeatmap[] IBeatmapset.Beatmaps
    {
        get => Beatmaps;
        set => Beatmaps = value.Cast<Beatmap>().ToArray();
    }
}