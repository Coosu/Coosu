using System;

namespace Coosu.Storyboard.Common
{
    public interface IEvent : ITimingAdjustable, ICloneable
    {
        event Action? TimingChanged;
        EventType EventType { get; }
        float StartTime { get; set; }
        float EndTime { get; set; }
    }
}