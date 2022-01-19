using System;

namespace Coosu.Beatmap.Configurable
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class SectionPropertyAttribute : Attribute
    {
        public string? Name { get; }

        public SectionPropertyAttribute() : this(null)
        {
        }

        public SectionPropertyAttribute(string? name)
        {
            var trimmed = name?.Trim();
            Name = trimmed == string.Empty ? null : trimmed;
        }
    }
}