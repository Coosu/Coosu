using System.Collections.Generic;
using System.Linq;

namespace Coosu.Storyboard.Common
{
    public sealed class EventSequenceComparer : IComparer<IKeyEvent>
    {
        private EventSequenceComparer() { }

        public int Compare(IKeyEvent? x, IKeyEvent? y)
        {
            if (y == null && x == null)
                return 0;
            if (y == null)
                return 1;
            if (x == null)
                return -1;
            if (x.StartTime > y.StartTime)
                return 1;
            if (x.StartTime < y.StartTime)
                return -1;
            if (x.EndTime > y.EndTime)
                return 1;
            if (x.EndTime < y.EndTime)
                return -1;
            if (x.EventType > y.EventType)
                return 1;
            if (x.EventType < y.EventType)
                return -1;
            if (x.Values.SequenceEqual(y.Values))
                return 0;
            return 1; // ensure object can be insert in order.
            //return 0;
        }

        public static EventSequenceComparer Instance { get; } = new();
    }
}