using System;

namespace OSharp.Shared.Mathematics
{
    public class RangeValueObject<T> where T : IComparable
    {
        public T StartTime { get; set; }
        public T EndTime { get; set; }

        public RangeValueObject(T startTime, T endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}