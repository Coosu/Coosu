using System;

namespace Coosu.Storyboard.Common
{
    public struct TimingPoint : IComparable<TimingPoint>
    {
        public TimingPoint(float timing, bool isStart) : this()
        {
            Timing = timing;
            IsStart = isStart;
        }

        public float Timing { get; set; }
        public bool IsStart { get; set; }
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
}