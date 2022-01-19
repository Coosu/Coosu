using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Sections.GamePlay;
using Coosu.Beatmap.Sections.Timing;

namespace Coosu.Beatmap.Sections
{
    [SectionProperty("General")]
    public sealed class GeneralSection : KeyValueSection
    {
        [SectionProperty("AudioFilename")]
        public string AudioFilename { get; set; } = "audio.mp3";

        [SectionProperty("AudioLeadIn")]
        public int AudioLeadIn { get; set; } = 0;

        [SectionProperty("PreviewTime")]
        public int PreviewTime { get; set; } = 0;

        [SectionProperty("Countdown")]
        [SectionBool(BoolParseType.ZeroOne)]
        public bool Countdown { get; set; } = true;

        [SectionProperty("SampleSet")]
        public TimingSamplesetType SampleSet { get; set; } = 0;

        [SectionProperty("StackLeniency")]
        public double StackLeniency { get; set; } = 0.7;

        [SectionProperty("Mode")]
        [SectionEnum(EnumParseOption.Index)]
        public GameMode Mode { get; set; } = 0;

        [SectionProperty("LetterboxInBreaks")]
        [SectionBool(BoolParseType.ZeroOne)]
        public bool LetterboxInBreaks { get; set; } = false;

        [SectionProperty("WidescreenStoryboard")]
        [SectionBool(BoolParseType.ZeroOne)]
        public bool WidescreenStoryboard { get; set; } = true;

        [SectionProperty("EpilepsyWarning")]
        [SectionBool(BoolParseType.ZeroOne)]
        public bool EpilepsyWarning { get; set; } = false;

        [SectionProperty("SkinPreference")]
        public string SkinPreference { get; set; }

        protected override string KeyValueFlag => ":";
        protected override bool TrimPairs => true;
    }
}
