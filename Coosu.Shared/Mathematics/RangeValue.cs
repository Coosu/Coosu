using System;

namespace Coosu.Shared.Mathematics;

public readonly struct RangeValue<T> where T : IComparable
{
    public T StartTime { get; }
    public T EndTime { get; }

    public RangeValue(T startTime, T endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
}