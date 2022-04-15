using Coosu.Database.DataTypes;

namespace OsuDbBenchmark;

public class SimpleBeatmap
{
    public string Artist { get; set; } = null!;
    public string ArtistUnicode { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string TitleUnicode { get; set; } = null!;
    public string Creator { get; set; } = null!;
    public string Version { get; set; } = null!;
    public string Source { get; set; } = null!;
    public string Tags { get; set; } = null!;
    public DbGameMode GameMode { get; set; }

    public string AudioFileName { get; set; } = null!;
    public string BeatmapFileName { get; set; } = null!;
    public string FolderName { get; set; } = null!;

    public double DefaultStarRatingStd { get; set; }
    public double DefaultStarRatingTaiko { get; set; }
    public double DefaultStarRatingCtB { get; set; }
    public double DefaultStarRatingMania { get; set; }

    public TimeSpan DrainTime { get; set; }
    public TimeSpan TotalTime { get; set; }
    public TimeSpan AudioPreviewTime { get; set; }
    public int BeatmapId { get; set; }
    public int BeatmapSetId { get; set; }
}