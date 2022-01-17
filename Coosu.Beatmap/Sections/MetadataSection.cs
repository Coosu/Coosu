using System.Collections.Generic;
using System.Text;
using Coosu.Beatmap.Configurable;

namespace Coosu.Beatmap.Sections
{
    [SectionProperty("Metadata")]
    public class MetadataSection : KeyValueSection
    {
        private string _artist;
        private string _title;

        [SectionProperty("Title")]
        public string Title
        {
            get => _title;
            set => _title = GetAsciiStr(value);
        }

        [SectionProperty("TitleUnicode")] public string TitleUnicode { get; set; }

        [SectionProperty("Artist")]
        public string Artist
        {
            get => _artist;
            set => _artist = GetAsciiStr(value);
        }

        [SectionProperty("ArtistUnicode")] public string ArtistUnicode { get; set; }
        [SectionProperty("Creator")] public string Creator { get; set; }
        [SectionProperty("Version")] public string Version { get; set; }
        [SectionProperty("Source")] public string Source { get; set; }

        [SectionProperty("Tags")]
        [SectionConverter(typeof(SplitConverter), " ")]
        public List<string> TagList { get; set; }

        [SectionProperty("BeatmapID")] public int BeatmapId { get; set; } = -1;
        [SectionProperty("BeatmapSetID")] public int BeatmapSetId { get; set; } = -1;

        [SectionIgnore]
        public MetaString TitleMeta => new MetaString(Title, TitleUnicode);
        [SectionIgnore]
        public MetaString ArtistMeta => new MetaString(Artist, ArtistUnicode);

        public string ToSerializedString(string newDiffName)
        {
            if (newDiffName == null)
                return ToSerializedString();

            var clonedSection = (MetadataSection)this.MemberwiseClone();
            clonedSection.Version = newDiffName;
            return clonedSection.ToSerializedString();
        }

        private static string GetAsciiStr(string value)
        {
            if (value == null) return null;

            var sb = new StringBuilder();
            foreach (var c in value)
            {
                if (c >= 32 && c <= 126)
                    sb.Append(c);
            }

            return sb.Length == 0 ? null : sb.ToString();
        }
    }
}
