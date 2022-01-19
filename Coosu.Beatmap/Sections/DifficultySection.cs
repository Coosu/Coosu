using Coosu.Beatmap.Configurable;

namespace Coosu.Beatmap.Sections
{
    [SectionProperty("Difficulty")]
    public sealed class DifficultySection : KeyValueSection
    {
        [SectionProperty("HPDrainRate")]
        public double HpDrainRate { get; set; } = 5;
        [SectionProperty("CircleSize")]
        public double CircleSize { get; set; } = 5;
        [SectionProperty("OverallDifficulty")]
        public double OverallDifficulty { get; set; } = 5;
        [SectionProperty("ApproachRate")]
        public double ApproachRate { get; set; } = 5;
        [SectionProperty("SliderMultiplier")]
        public double SliderMultiplier { get; set; } = 1.0;
        [SectionProperty("SliderTickRate")]
        public double SliderTickRate { get; set; } = 1.0;
    }
}
