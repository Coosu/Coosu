using System.Collections.Generic;
using Coosu.Beatmap.Configurable;

namespace Coosu.Beatmap.Sections
{
    [SectionProperty("Editor")]
    public sealed class EditorSection : KeyValueSection
    {
        /// <summary>
        /// Time in milliseconds of bookmarks
        /// </summary>
        [SectionProperty("Bookmarks", Default = SectionPropertyAttribute.DefaultValue)]
        [SectionConverter(typeof(Int32SplitConverter), ',')]
        public List<int> Bookmarks { get; set; } = new();

        /// <summary>
        /// Distance snap multiplier
        /// </summary>
        [SectionProperty("DistanceSpacing")]
        public double DistanceSpacing { get; set; } = 1;

        /// <summary>
        /// Beat snap divisor
        /// </summary>
        [SectionProperty("BeatDivisor")]
        public int BeatDivisor { get; set; } = 4;

        /// <summary>
        /// Grid size
        /// </summary>
        [SectionProperty("GridSize")]
        public int GridSize { get; set; } = 4;

        /// <summary>
        /// Scale factor for the object timeline
        /// </summary>
        [SectionProperty("TimelineZoom")]
        public double TimelineZoom { get; set; } = 1;

        protected override string KeyValueFlag => ": ";
    }
}
