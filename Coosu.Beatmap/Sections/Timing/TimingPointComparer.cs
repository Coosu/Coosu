using System.Collections.Generic;

namespace Coosu.Beatmap.Sections.Timing;

public sealed class TimingPointComparer : IComparer<TimingPoint>
{
    private TimingPointComparer()
    {
    }

    public static IComparer<TimingPoint> Instance { get; } = new TimingPointComparer();

    public int Compare(TimingPoint? x, TimingPoint? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y)) return 1;
        if (ReferenceEquals(null, x)) return -1;

        var eq = x.Offset.CompareTo(y.Offset);
        if (eq != 0) return eq;
        if (x.IsInherit && !y.IsInherit) return 1;
        if (y.IsInherit && !x.IsInherit) return -1;
        return 0;
    }
}