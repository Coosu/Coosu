using System;

namespace Coosu.Api.V2.ResponseModels
{
    public interface IBeatmap
    {
        long BeatmapsetId { get; set; }
        double DifficultyRating { get; set; }
        long Id { get; set; }
        string Mode { get; set; }
        string Status { get; set; }
        long TotalLength { get; set; }
        long UserId { get; set; }
        string Version { get; set; }
        double Od { get; set; }
        double Ar { get; set; }
        double? Bpm { get; set; }
        bool Convert { get; set; }
        long CountCircles { get; set; }
        long CountSliders { get; set; }
        long CountSpinners { get; set; }
        double Cs { get; set; }
        object DeletedAt { get; set; }
        double Hp { get; set; }
        long HitLength { get; set; }
        bool IsScoreable { get; set; }
        DateTimeOffset LastUpdated { get; set; }
        long ModeInt { get; set; }
        long Passcount { get; set; }
        long Playcount { get; set; }
        long Ranked { get; set; }
        string? Url { get; set; }
        string Checksum { get; set; }
        long? MaxCombo { get; set; }
    }
}