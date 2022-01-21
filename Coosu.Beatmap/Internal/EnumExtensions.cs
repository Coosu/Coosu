using System;
using Coosu.Beatmap.Sections.HitObject;

namespace Coosu.Beatmap.Internal
{
    public static class EnumExtensions
    {
        internal static SliderType SliderFlagToEnum(this char flag)
        {
            return flag switch
            {
                'L' => SliderType.Linear,
                'P' => SliderType.Perfect,
                'B' => SliderType.Bezier,
                'C' => SliderType.Catmull,
                _ => throw new ArgumentOutOfRangeException(nameof(flag), flag, null)
            };
        }

        internal static char ToSliderFlag(this SliderType sliderType)
        {
            return sliderType switch
            {
                SliderType.Linear => 'L',
                SliderType.Perfect => 'P',
                SliderType.Bezier => 'B',
                SliderType.Catmull => 'C',
                _ => throw new ArgumentOutOfRangeException(nameof(sliderType), sliderType, null)
            };
        }

        internal static T ParseToEnum<T>(this string value) where T : struct
        {
#if NETCOREAPP3_1_OR_GREATER
            return Enum.Parse<T>(value);
#else
            return (T)Enum.Parse(typeof(T), value);
#endif
        }
    }
}
