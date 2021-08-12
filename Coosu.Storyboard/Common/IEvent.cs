namespace Coosu.Storyboard.Common
{
    public interface IEvent : ITimingAdjustable
    {
        EventType EventType { get; }
        float StartTime { get; }
        float EndTime { set; get; }
    }
}