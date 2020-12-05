using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Coosu.Beatmap.Internal
{
    internal static class NumericExtension
    {
        public static string ToInvariantString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
