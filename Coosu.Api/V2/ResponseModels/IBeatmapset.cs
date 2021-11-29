using System;

namespace Coosu.Api.V2.ResponseModels
{
    public interface IBeatmapset
    {
        string Artist { get; set; }
        string ArtistUnicode { get; set; }
        BeatmapCovers Covers { get; set; }
        string Creator { get; set; }
        long FavouriteCount { get; set; }
        object Hype { get; set; }
        long Id { get; set; }
        bool Nsfw { get; set; }
        long PlayCount { get; set; }
        string PreviewUrl { get; set; }
        string Source { get; set; }
        string Status { get; set; }
        string Title { get; set; }
        string TitleUnicode { get; set; }
        long? TrackId { get; set; }
        long UserId { get; set; }
        bool Video { get; set; }
        Availability Availability { get; set; }
        double? Bpm { get; set; }
        bool CanBeHyped { get; set; }
        bool DiscussionEnabled { get; set; }
        bool DiscussionLocked { get; set; }
        bool IsScoreable { get; set; }
        DateTimeOffset LastUpdated { get; set; }
        string? LegacyThreadUrl { get; set; }
        NominationsSummary NominationsSummary { get; set; }
        long Ranked { get; set; }
        DateTimeOffset? RankedDate { get; set; }
        bool Storyboard { get; set; }
        DateTimeOffset SubmittedDate { get; set; }
        string Tags { get; set; }
        IBeatmap[] Beatmaps { get; set; }
    }
}