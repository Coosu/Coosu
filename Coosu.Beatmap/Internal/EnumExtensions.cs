using System;
using Coosu.Beatmap.Sections.HitObject;

namespace Coosu.Beatmap.Internal;

public static class EnumExtensions
{
    internal static SliderType SliderFlagToEnum(this ReadOnlySpan<char> flag)
    {
        return flag switch
        {
            "L" => SliderType.Linear,
            "P" => SliderType.Perfect,
            "B" => SliderType.Bezier,
            "B4" => SliderType.Bezier4,
#pragma warning disable CS0618
            "C" => SliderType.Catmull,
#pragma warning restore CS0618
            _ => throw new ArgumentOutOfRangeException(nameof(flag), flag.ToString(), null)
        };
    }

    internal static SliderType SliderFlagToEnum(this char flag)
    {
        return flag switch
        {
            'L' => SliderType.Linear,
            'P' => SliderType.Perfect,
            'B' => SliderType.Bezier,
#pragma warning disable CS0618
            'C' => SliderType.Catmull,
#pragma warning restore CS0618
            _ => throw new ArgumentOutOfRangeException(nameof(flag), flag, null)
        };
    }

    internal static string ToSliderFlag(this SliderType sliderType)
    {
        return sliderType switch
        {
            SliderType.Linear => "L",
            SliderType.Perfect => "P",
            SliderType.Bezier => "B",
            SliderType.Bezier4 => "B4",
#pragma warning disable CS0618
            SliderType.Catmull => "C",
#pragma warning restore CS0618
            _ => throw new ArgumentOutOfRangeException(nameof(sliderType), sliderType, null)
        };
    }
}