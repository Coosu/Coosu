using Coosu.Shared.Numerics;

namespace Coosu.Shared
{
    public static class NumericExtensions
    {
        public static string ToEnUsFormatString(this float value)
        {
            return value.ToString(ParseHelper.EnUsNumberFormat);
        }

        public static string ToEnUsFormatString(this double value)
        {
            return value.ToString(ParseHelper.EnUsNumberFormat);
        }
    }
}