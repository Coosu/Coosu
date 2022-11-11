using System.Collections.Generic;

namespace Coosu.Storyboard.Common;

public class TimingPointComparer : IComparer<TimeOffset>
{
    private TimingPointComparer()
    {
    }

    public static IComparer<TimeOffset> Instance { get; } = new TimingPointComparer();

    public int Compare(TimeOffset x, TimeOffset y)
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