namespace Coosu.Storyboard.Events
{
    public interface IEvent : ITimingAdjustable
    {
        float StartTime { get; }
        float EndTime { get; }
    }
}