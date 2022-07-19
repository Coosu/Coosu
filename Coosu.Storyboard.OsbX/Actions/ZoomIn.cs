using System.Collections.Generic;
using Coosu.Storyboard.Easing;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.OsbX.Actions
{
    public class ZoomIn : BasicEvent
    {
        public ZoomIn()
        {
        }

        public ZoomIn(EasingFunctionBase easing, double startTime, double endTime, List<double> values)
            : base(easing, startTime, endTime, values)
        {
        }

        public override EventType EventType { get; } = new("ZI", 1, 11);
    }
}
