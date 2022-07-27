using System;

namespace Coosu.Beatmap.Configurable;

public sealed class SectionEnumAttribute : Attribute
{
    public EnumParseOption Option { get; }

    public SectionEnumAttribute(EnumParseOption option)
    {
        Option = option;
    }
}