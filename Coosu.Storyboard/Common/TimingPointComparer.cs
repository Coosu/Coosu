using System.Collections.Generic;

namespace Coosu.Storyboard.Common;

public class TimingPointComparer : IComparer<TimingPoint>
{
    private TimingPointComparer()
    {
    }

    public static IComparer<TimingPoint> Instance { get; } = new TimingPointComparer();

    public int Compare(TimingPoint x, TimingPoint y)
    {
        var val = x.Timing.CompareTo(y.Timing);
        if (val != 0)
            return val;
        if (x.IsStart && !y.IsStart)
            return 1;
        if (!x.IsStart && y.IsStart)
            return -1;

        return 0;
    }
}