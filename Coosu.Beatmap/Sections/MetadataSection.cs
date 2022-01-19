using System.Collections.Generic;
using System.Text;
using Coosu.Beatmap.Configurable;

namespace Coosu.Beatmap.Sections
{
    [SectionProperty("Metadata")]
    public sealed class MetadataSection : KeyValueSection
    {
        private string _artist = null!;
        private string _title = null!;

        [SectionProperty("Title")]
        public string Title
        {
            get => _title;
            set => _title = MetaString.GetAsciiStr(value);
        }

        [SectionProperty("TitleUnicode")] public string? TitleUnicode { get; set; }

        [SectionProperty("Artist")]
        public string Artist
        {
            get => _artist;
            set => _artist = MetaString.GetAsciiStr(value);
        }

        [SectionProperty("ArtistUnicode")] public string? ArtistUnicode { get; set; }
        [SectionProperty("Creator")] public string? Creator { get; set; }
        [SectionProperty("Version")] public string? Version { get; set; }
        [SectionProperty("Source")] public string? Source { get; set; }

        [SectionProperty("Tags")]
        [SectionConverter(typeof(SplitConverter), ' ')]
        public List<string> TagList { get; set; } = new();

        [SectionProperty("BeatmapID")] public int BeatmapId { get; set; } = -1;
        [SectionProperty("BeatmapSetID")] public int BeatmapSetId { get; set; } = -1;

        [SectionIgnore]
        public MetaString TitleMeta => new(Title, TitleUnicode);
        [SectionIgnore]
        public MetaString ArtistMeta => new(Artist, ArtistUnicode);

        public string ToSerializedString(string? newDiffName)
        {
            if (newDiffName == null)
                return ToSerializedString();

            var clonedSection = (MetadataSection)this.MemberwiseClone();
            clonedSection.Version = newDiffName;
            return clonedSection.ToSerializedString();
        }
    }
}
