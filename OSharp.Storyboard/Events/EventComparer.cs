﻿using System.Collections.Generic;
using System.Linq;

namespace OSharp.Storyboard.Events
{
    public class EventComparer : IComparer<CommonEvent>
    {
        public int Compare(CommonEvent x, CommonEvent y)
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
            if (x.Start.SequenceEqual(y.Start) &&
                x.End.SequenceEqual(y.End))
                return 0;
            return 1; // ensure object can be insert in order.
            //return 0;
        }
    }
}