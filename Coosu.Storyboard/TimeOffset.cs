using System;

namespace Coosu.Storyboard;

public readonly struct TimeOffset : IComparable<TimeOffset>
{
    public TimeOffset(double timing, bool isStart)
    {
        Timing = timing;
        IsStart = isStart;
    }

    public bool IsStart { get; }
    public double Timing { get; }

    public int CompareTo(TimeOffset other)
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