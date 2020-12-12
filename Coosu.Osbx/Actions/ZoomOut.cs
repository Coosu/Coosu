using Coosu.Storyboard.Events;

namespace Coosu.Osbx.Actions
{
    public class ZoomOut : CommonEvent
    {
        public ZoomOut()
        {
        }

        public ZoomOut(EasingType easing, float startTime, float endTime, float[] start, float[] end) : base(easing, startTime, endTime, start, end)
        {
        }

        public override EventType EventType => "ZO";
    }
}