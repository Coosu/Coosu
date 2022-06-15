using System;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Sections.Timing;

namespace Coosu.Beatmap.Extensions
{
    public static class HitsoundToStringExtensions
    {
        public static string ToHitsoundString(this TimingSamplesetType type)
        {
            return type switch
            {
                TimingSamplesetType.Soft => "soft",
                TimingSamplesetType.Drum => "drum",
                TimingSamplesetType.None => "normal",
                TimingSamplesetType.Normal => "normal",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public static string? ToHitsoundString(this ObjectSamplesetType type, string? sample)
        {
            return type switch
            {
                ObjectSamplesetType.Soft => "soft",
                ObjectSamplesetType.Drum => "drum",
                ObjectSamplesetType.Normal => "normal",
                ObjectSamplesetType.Auto => sample,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}