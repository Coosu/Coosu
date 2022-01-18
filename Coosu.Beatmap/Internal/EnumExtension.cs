using System;
using Coosu.Beatmap.Sections.HitObject;

namespace Coosu.Beatmap.Internal
{
    public static class EnumExtension
    {
        internal static T ParseToEnum<T>(this string value)
        {
            if (typeof(T) == typeof(SliderType))
            {
                if (value == "L")
                    value = "Linear";
                else if (value == "P")
                    value = "Perfect";
                else if (value == "B")
                    value = "Bezier";
                else if (value == "C")
                    value = "Catmull";
            }

            return (T)Enum.Parse(typeof(T), value);
        }

        internal static string ParseToCode(this SliderType sliderType)
        {
            return sliderType switch
            {
                SliderType.Linear => "L",
                SliderType.Perfect => "P",
                SliderType.Bezier => "B",
                SliderType.Catmull => "C",
                _ => throw new ArgumentOutOfRangeException(nameof(sliderType), sliderType, null)
            };
        }
    }
}
