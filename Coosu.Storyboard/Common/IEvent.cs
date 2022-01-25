using System;

namespace Coosu.Storyboard.Common
{
    public interface IEvent : ITimingAdjustable, ICloneable
    {
        EventType EventType { get; }
        float StartTime { get; }
        float EndTime { set; get; }
    }
}