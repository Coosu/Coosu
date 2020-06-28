using System;

namespace OSharp.Beatmap.Configurable
{
    public class SectionEnumAttribute : Attribute
    {
        public EnumParseOption Option { get; }

        public SectionEnumAttribute(EnumParseOption option)
        {
            Option = option;
        }
    }
}