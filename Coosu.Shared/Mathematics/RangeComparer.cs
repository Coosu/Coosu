using System;
using System.Collections.Generic;

namespace Coosu.Shared.Mathematics;

public class RangeComparer<T> : IComparer<RangeValue<T>> where T : IComparable
{
    public int Compare(RangeValue<T> x, RangeValue<T> y)
    {
        var value = x.StartTime.CompareTo(y.StartTime);
        return value == 0
            ? x.EndTime.CompareTo(y.EndTime)
            : value;
    }
}