using Coosu.Beatmap.Configurable;

namespace Coosu.Beatmap
{
    public sealed class LocalOsuFile : OsuFile
    {
        [SectionIgnore]
        public string OriginPath { get; internal set; }
    }
}