using System.Globalization;

namespace Coosu.Shared
{
    public static class NumericExtensions
    {
        public static string ToIcString(this float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToIcString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}