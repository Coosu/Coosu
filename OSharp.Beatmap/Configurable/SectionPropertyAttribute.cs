using System;

namespace OSharp.Beatmap.Configurable
{
    public class SectionPropertyAttribute : Attribute
    {
        public string Name { get; }

        public SectionPropertyAttribute() : this(null)
        {
        }

        public SectionPropertyAttribute(string name)
        {
            var tmp = name?.Trim();
            if (tmp == "")
                Name = null;
            else
                Name = tmp;
        }
    }
}