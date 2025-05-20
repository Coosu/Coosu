using Coosu.Beatmap.Configurable;
using Coosu.Shared.Numerics;

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
[SectionProperty("Colours")]
public sealed class ColorSection : KeyValueSection
{
    [SectionProperty("Combo1", Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo1 { get; set; }

    [SectionProperty("Combo2", Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo2 { get; set; }

    [SectionProperty("Combo3", Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo3 { get; set; }

    [SectionProperty("Combo4", Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo4 { get; set; }

    [SectionProperty("Combo5", Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo5 { get; set; }

    [SectionProperty("Combo6", Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo6 { get; set; }

    [SectionProperty("Combo7", Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo7 { get; set; }

    [SectionProperty("Combo8", Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo8 { get; set; }

    /// <summary>
    /// Additive slider track color
    /// </summary>
    [SectionProperty("SliderTrackOverride", Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? SliderTrackOverride { get; set; }

    /// <summary>
    /// Slider border color
    /// </summary>
    [SectionProperty("SliderBorder", Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? SliderBorder { get; set; }

    protected override FlagRule FlagRule { get; } = FlagRules.SpaceColonSpace;
}