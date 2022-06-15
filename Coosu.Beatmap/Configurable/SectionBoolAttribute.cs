using System;

namespace Coosu.Beatmap.Configurable
{
    public sealed class SectionBoolAttribute : Attribute
    {
        public BoolParseType Type { get; }

        public SectionBoolAttribute(BoolParseType type)
        {
            Type = type;
        }
    }
}