using System;

namespace Coosu.Beatmap.Configurable
{
    public class SectionBoolAttribute : Attribute
    {
        public BoolParseOption Option { get; }

        public SectionBoolAttribute(BoolParseOption option)
        {
            Option = option;
        }
    }
}