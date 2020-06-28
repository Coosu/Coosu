using System.Collections.Generic;

namespace OSharp.Storyboard.Common
{
    public class TimingPointComparer : IComparer<TimingPoint>
    {
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
}