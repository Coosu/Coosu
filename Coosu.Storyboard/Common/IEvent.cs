using System;

namespace Coosu.Storyboard.Common
{
    public interface IEvent : ITimingAdjustable, ICloneable
    {
        event Action? TimingChanged;
        EventType EventType { get; }
        double StartTime { get; set; }
        double EndTime { get; set; }
    }
}