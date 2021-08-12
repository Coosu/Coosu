using System.Collections.Generic;
using System.Linq;

namespace Coosu.Storyboard.Utils
{
    public static class NumericHelper
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
    }
}
