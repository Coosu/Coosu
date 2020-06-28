using System.Globalization;

namespace OSharp.Beatmap.Internal
{
    internal static class NumericExtension
    {
        public static string ToInvariantString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
