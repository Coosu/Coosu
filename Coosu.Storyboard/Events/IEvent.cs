namespace Coosu.Storyboard.Events
{
    public interface IEvent : ITimingAdjustable
    {
        EventType EventType { get; }
        float StartTime { get; }
        float EndTime { set; get; }
    }
}