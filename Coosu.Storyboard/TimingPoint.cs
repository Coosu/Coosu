using System;

namespace Coosu.Storyboard;

public sealed class TimingPoint : IComparable<TimingPoint>
{
    public TimingPoint(double timing, bool isStart)
    {
        Timing = timing;
        IsStart = isStart;
    }

    public bool IsStart { get; set; }
    public double Timing { get; set; }

    public int CompareTo(TimingPoint other)
    {
        var val = Timing.CompareTo(other.Timing);
        if (val != 0)
            return val;
        if (IsStart && !other.IsStart)
            return 1;
        if (!IsStart && other.IsStart)
            return -1;

        return 0;
    }
}