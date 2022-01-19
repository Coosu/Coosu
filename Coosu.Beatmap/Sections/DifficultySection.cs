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
        public float SliderMultiplier { get; set; } = 1.0f;
        [SectionProperty("SliderTickRate")]
        public float SliderTickRate { get; set; } = 1.0f;
    }
}
