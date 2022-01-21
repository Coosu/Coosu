using Coosu.Beatmap.Configurable;
using Coosu.Beatmap.Sections.GamePlay;
using Coosu.Beatmap.Sections.Timing;

namespace Coosu.Beatmap.Sections
{
    [SectionProperty("General")]
    public sealed class GeneralSection : KeyValueSection
    {
        /// <summary>
        /// Location of the audio file relative to the current folder
        /// </summary>
        [SectionProperty("AudioFilename")]
        public string? AudioFilename { get; set; }

        /// <summary>
        /// Milliseconds of silence before the audio starts playing
        /// </summary>
        [SectionProperty("AudioLeadIn")]
        public int AudioLeadIn { get; set; } = 0;

        /// <summary>
        /// Time in milliseconds when the audio preview should start
        /// </summary>
        [SectionProperty("PreviewTime")]
        public int PreviewTime { get; set; } = -1;

        /// <summary>
        /// Speed of the countdown before the first hit object
        /// </summary>
        [SectionProperty("Countdown")]
        [SectionEnum(EnumParseOption.Index)]
        public CountDownType Countdown { get; set; } = CountDownType.Normal;

        /// <summary>
        /// Sample set that will be used if timing points do not override it
        /// </summary>
        [SectionProperty("SampleSet")]
        public TimingSamplesetType SampleSet { get; set; } = 0;

        /// <summary>
        /// Multiplier for the threshold in time where hit objects placed close together stack (0–1)
        /// </summary>
        [SectionProperty("StackLeniency")]
        public double StackLeniency { get; set; } = 0.7;

        /// <summary>
        /// Game mode
        /// </summary>
        [SectionProperty("Mode")]
        [SectionEnum(EnumParseOption.Index)]
        public GameMode Mode { get; set; } = 0;

        /// <summary>
        /// Whether or not breaks have a letterboxing effect
        /// </summary>
        [SectionProperty("LetterboxInBreaks")]
        [SectionBool(BoolParseType.ZeroOne)]
        public bool LetterboxInBreaks { get; set; } = false;

        /// <summary>
        /// Whether or not the storyboard can use the user's skin images
        /// </summary>
        [SectionProperty("UseSkinSprites", Default = false)]
        [SectionBool(BoolParseType.ZeroOne)]
        public bool UseSkinSprites { get; set; } = false;

        /// <summary>
        /// Draw order of hit circle overlays compared to hit numbers
        /// </summary>
        [SectionProperty("OverlayPosition", Default = OverlayPosition.NoChange)]
        public OverlayPosition OverlayPosition { get; set; } = OverlayPosition.NoChange;

        /// <summary>
        /// Preferred skin to use during gameplay
        /// </summary>
        [SectionProperty("SkinPreference", Default = SectionPropertyAttribute.DefaultValue)]
        public string? SkinPreference { get; set; }

        /// <summary>
        /// Whether or not a warning about flashing colors should be shown at the beginning of the map
        /// </summary>
        [SectionProperty("EpilepsyWarning")]
        [SectionBool(BoolParseType.ZeroOne)]
        public bool EpilepsyWarning { get; set; } = false;

        /// <summary>
        /// Time in beats that the countdown starts before the first hit object
        /// </summary>
        [SectionProperty("CountdownOffset", Default = 0)]
        public int CountdownOffset { get; set; } = 0;

        /// <summary>
        /// Whether or not the "N+1" style key layout is used for osu!mania
        /// </summary>
        [SectionProperty("SpecialStyle", Default = false)]
        [SectionBool(BoolParseType.ZeroOne)]
        public bool SpecialStyle { get; set; } = false;

        /// <summary>
        /// Whether or not the storyboard allows widescreen viewing
        /// </summary>
        [SectionProperty("WidescreenStoryboard")]
        [SectionBool(BoolParseType.ZeroOne)]
        public bool WidescreenStoryboard { get; set; } = false;

        /// <summary>
        /// Whether or not sound samples will change rate when playing with speed-changing mods
        /// </summary>
        [SectionProperty("SamplesMatchPlaybackRate", Default = false)]
        [SectionBool(BoolParseType.ZeroOne)]
        public bool SamplesMatchPlaybackRate { get; set; } = false;

        protected override string KeyValueFlag => ": ";
    }
}
