using System;
using System.Collections.Generic;

namespace Coosu.Shared.Mathematics
{
    public class RangeObjectComparer<T> : IComparer<RangeValueObject<T>> where T : IComparable
    {
        public int Compare(RangeValueObject<T>? x, RangeValueObject<T>? y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return 1;
            if (y == null) return 0;
            var value = x.StartTime.CompareTo(y.StartTime);
            return value == 0
                ? x.EndTime.CompareTo(y.EndTime)
                : value;
        }
    }
}