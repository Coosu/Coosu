using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.OsbX.Actions
{
    public class ZoomIn : CommonEvent
    {
        public ZoomIn()
        {
        }
        
        public ZoomIn(EasingFunctionBase easing, double startTime, double endTime, double[] start, double[] end)
            : base(easing, startTime, endTime, start, end)
        {
        }

        public override EventType EventType { get; } = new("ZI", 1, 11);
    }
}
