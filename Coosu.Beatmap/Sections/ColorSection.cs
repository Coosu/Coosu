﻿using Coosu.Beatmap.Configurable;
using Coosu.Shared.Numerics;

namespace Coosu.Beatmap.Sections;

[SectionProperty("Colours")]
public sealed class ColorSection : KeyValueSection
{
    [SectionProperty(Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo1 { get; set; }

    [SectionProperty(Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo2 { get; set; }

    [SectionProperty(Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo3 { get; set; }

    [SectionProperty(Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo4 { get; set; }

    [SectionProperty(Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo5 { get; set; }

    [SectionProperty(Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo6 { get; set; }

    [SectionProperty(Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo7 { get; set; }

    [SectionProperty(Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? Combo8 { get; set; }

    /// <summary>
    /// Additive slider track color
    /// </summary>
    [SectionProperty(Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? SliderTrackOverride { get; set; }

    /// <summary>
    /// Slider border color
    /// </summary>
    [SectionProperty(Default = SectionPropertyAttribute.DefaultValue)]
    [SectionConverter(typeof(ColorConverter))]
    public ReadyOnlyVector3<byte>? SliderBorder { get; set; }

    protected override FlagRule FlagRule { get; } = FlagRules.SpaceColonSpace;
}