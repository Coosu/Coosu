using System;

namespace Coosu.Storyboard.Events
{
    public static class ParameterExtension
    {
        public static string ToShortString(this ParameterType pType)
        {
            switch (pType)
            {
                case ParameterType.Horizontal:
                    return "H";
                case ParameterType.Vertical:
                    return "V";
                case ParameterType.Additive:
                    return "A";
                default:
                    throw new ArgumentOutOfRangeException(nameof(pType), pType, null);
            }
        }

        public static ParameterType ToParameterEnum(this string str)
        {
            switch (str)
            {
                case "H":
                    return ParameterType.Horizontal;
                case "V":
                    return ParameterType.Vertical;
                case "A":
                    return ParameterType.Additive;
                default:
                    throw new ArgumentOutOfRangeException(nameof(str), str, null);
            }
        }
    }
}