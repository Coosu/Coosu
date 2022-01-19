using System.Collections.Generic;

namespace Coosu.Beatmap.Sections.Timing
{
    public sealed class TimingPointComparer : IComparer<TimingPoint>
    {
        public int Compare(TimingPoint? x, TimingPoint? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return x.Offset.CompareTo(y.Offset);
        }
    }
}