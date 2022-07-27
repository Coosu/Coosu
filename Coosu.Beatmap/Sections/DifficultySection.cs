using Coosu.Beatmap.Configurable;

namespace Coosu.Beatmap.Sections;

[SectionProperty("Difficulty")]
public sealed class DifficultySection : KeyValueSection
{
    [SectionProperty("HPDrainRate", UseSpecificFormat = true)]
    public float HpDrainRate { get; set; } = 5;
    [SectionProperty("CircleSize", UseSpecificFormat = true)]
    public float CircleSize { get; set; } = 5;
    [SectionProperty("OverallDifficulty", UseSpecificFormat = true)]
    public float OverallDifficulty { get; set; } = 5;
    [SectionProperty("ApproachRate", UseSpecificFormat = true)]
    public float ApproachRate { get; set; } = 5;
    [SectionProperty("SliderMultiplier", UseSpecificFormat = true)]
    public float SliderMultiplier { get; set; } = 1.0f;
    [SectionProperty("SliderTickRate", UseSpecificFormat = true)]
    public float SliderTickRate { get; set; } = 1.0f;
}