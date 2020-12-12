using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Coosu.Storyboard.Utils
{
    public static class NumericUtility
    {
        public static float GetMaxValue(params IEnumerable<float>[] floatLists)
        {
            return floatLists
                .Where(floatList => floatList.Any())
                .Select(floatList => floatList.Max())
                .Concat(new[] { float.MinValue })
                .Max();
        }

        public static float GetMinValue(params IEnumerable<float>[] floatLists)
        {
            return floatLists
                .Where(floatList => floatList.Any())
                .Select(floatList => floatList.Min())
                .Concat(new[] { float.MaxValue })
                .Min();
        }

        public static string ToInvariantString(this float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToInvariantString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
