namespace Coosu.Storyboard.Common
{
    public interface IEvent : ITimingAdjustable
    {
        EventType EventType { get; }
        double StartTime { get; }
        double EndTime { set; get; }
    }
}