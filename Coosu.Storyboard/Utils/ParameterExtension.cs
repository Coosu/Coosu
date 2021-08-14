using System;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Utils
{
    public static class ParameterExtension
    {
        public static string ToShortString(this ParameterType pType)
        {
            return pType switch
            {
                ParameterType.Horizontal => "H",
                ParameterType.Vertical => "V",
                ParameterType.Additive => "A",
                _ => throw new ArgumentOutOfRangeException(nameof(pType), pType, null)
            };
        }

        public static ParameterType ToParameterEnum(this string str)
        {
            return str switch
            {
                "H" => ParameterType.Horizontal,
                "V" => ParameterType.Vertical,
                "A" => ParameterType.Additive,
                _ => throw new ArgumentOutOfRangeException(nameof(str), str, null)
            };
        }
    }
}