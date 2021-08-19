using System;

namespace Coosu.Storyboard.Common
{
    public interface IEvent : ITimingAdjustable, ICloneable
    {
        EventType EventType { get; }
        double StartTime { get; }
        double EndTime { set; get; }
    }
}