using System;
using Coosu.Database.Annotations;

namespace Coosu.Database.DataTypes;

public class Score
{
    public DbGameMode GameMode { get; set; }
    public int ScoreVersion { get; set; }
    public string BeatmapHash { get; set; } = null!;
    public string Player { get; set; } = null!;
    public string ReplayHash { get; set; } = null!;

    /// <summary>
    /// Number of 300's
    /// </summary>
    public short Count300 { get; set; }

    /// <summary>
    /// Number of 100's in osu!, 150's in osu!taiko, 100's in osu!catch, 100's in osu!mania
    /// </summary>
    public short Count100 { get; set; }

    /// <summary>
    /// Number of 50's in osu!, small fruit in osu!catch, 50's in osu!mania
    /// </summary>
    public short Count50 { get; set; }

    /// <summary>
    /// Number of Gekis in osu!, Max 300's in osu!mania
    /// </summary>
    public short CountGeki { get; set; }

    /// <summary>
    /// Number of Katus in osu!, 200's in osu!mania
    /// </summary>
    public short CountKatu { get; set; }
    public short CountMiss { get; set; }
    public int ReplayScore { get; set; }
    public short MaxCombo { get; set; }
    public bool IsPerfect { get; set; }
    public Mods Mods { get; set; }
    public string LifeBarGraph { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public int ReplayLength { get; set; }
    public long OnlineScoreId { get; set; }

    [StructureIgnoreWhen(nameof(Mods), StructureIgnoreWhenAttribute.Condition.NotContains, Mods.TargetPractice)]
    public double TargetPracticeAccuracy { get; set; }
}