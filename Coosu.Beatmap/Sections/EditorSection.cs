using System.Collections.Generic;
using Coosu.Beatmap.Configurable;

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Coosu.Beatmap.Sections;

#if NET6_0_OR_GREATER
[DynamicallyAccessedMembers(
    DynamicallyAccessedMemberTypes.PublicProperties |
    DynamicallyAccessedMemberTypes.NonPublicProperties |
    DynamicallyAccessedMemberTypes.PublicConstructors |
    DynamicallyAccessedMemberTypes.NonPublicConstructors)]
#endif
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
    [SectionProperty("DistanceSpacing", UseSpecificFormat = true)]
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
    [SectionProperty("TimelineZoom", UseSpecificFormat = true)]
    public double TimelineZoom { get; set; } = 1;

    protected override FlagRule FlagRule { get; } = FlagRules.ColonSpace;
}