using System.Collections.Generic;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.OsbX.Actions
{
    public class ZoomOut : BasicEvent
    {
        public ZoomOut()
        {
        }

        public ZoomOut(EasingFunctionBase easing, double startTime, double endTime, List<double> values)
            : base(easing, startTime, endTime, values)
        {
        }

        public override EventType EventType { get; } = new("ZO", 1, 12);
    }
}