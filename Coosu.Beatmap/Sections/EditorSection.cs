using System.Collections.Generic;
using Coosu.Beatmap.Configurable;

namespace Coosu.Beatmap.Sections
{
    [SectionProperty("Editor")]
    public class EditorSection : KeyValueSection
    {
        [SectionProperty("Bookmarks")]
        [SectionConverter(typeof(Int32SplitConverter), ",")]
        public List<int> Bookmarks { get; set; }

        [SectionProperty("DistanceSpacing")]
        public double DistanceSpacing { get; set; } = 1;

        [SectionProperty("BeatDivisor")]
        public int BeatDivisor { get; set; } = 4;

        [SectionProperty("GridSize")]
        public int GridSize { get; set; } = 4;

        [SectionProperty("TimelineZoom")]
        public double TimelineZoom { get; set; } = 1;

        protected override string KeyValueFlag => ": ";
    }
}
