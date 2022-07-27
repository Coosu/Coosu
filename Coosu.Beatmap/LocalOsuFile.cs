using Coosu.Beatmap.Configurable;

namespace Coosu.Beatmap;

public sealed class LocalOsuFile : OsuFile
{
    [SectionIgnore]
    public string? OriginalPath { get; internal set; }
}