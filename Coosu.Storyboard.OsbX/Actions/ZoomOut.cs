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

        public ZoomOut(EasingFunctionBase easing, float startTime, float endTime, List<float> values)
            : base(easing, startTime, endTime, values)
        {
        }

        public override EventType EventType { get; } = new("ZO", 1, 12);
    }
}