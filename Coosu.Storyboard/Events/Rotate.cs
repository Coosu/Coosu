using System;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events
{
    public class Rotate : BasicEvent
    {
        public override EventType EventType => EventTypes.Rotate;

        public double StartRotate
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public double EndRotate
        {
            get => End[0];
            set => End[0] = value;
        }

        public Rotate(EasingFunctionBase easing, double startTime, double endTime, double r1, double r2) :
            base(easing, startTime, endTime, new[] { r1 }, new[] { r2 })
        {
        }

        public Rotate(EasingFunctionBase easing, double startTime, double endTime, Span<double> start, Span<double> end)
            : base(easing, startTime, endTime, start, end)
        {
        }

        public Rotate()
        {
        }
    }
}
