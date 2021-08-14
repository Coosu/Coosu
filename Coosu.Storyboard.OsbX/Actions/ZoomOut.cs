using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.OsbX.Actions
{
    public class ZoomOut : CommonEvent
    {
        public ZoomOut()
        {
        }

        public ZoomOut(EasingType easing, double startTime, double endTime, double[] start, double[] end)
            : base(easing, startTime, endTime, start, end)
        {
        }

        public override EventType EventType { get; } = new("ZO", 1, 12);
    }
}