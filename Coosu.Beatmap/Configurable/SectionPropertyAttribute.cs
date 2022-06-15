using System;

namespace Coosu.Beatmap.Configurable
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public sealed class SectionPropertyAttribute : Attribute
    {
        public const double DefaultValue = double.MaxValue;
        public string? Name { get; }

        public object? Default { get; set; }

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