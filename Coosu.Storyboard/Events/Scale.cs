using System;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events
{
    public sealed class Scale : CommonEvent
    {
        public override EventType EventType => EventTypes.Scale;

        public double StartScale
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public double EndScale
        {
            get => End[0];
            set => End[0] = value;
        }

        public Scale(EasingFunctionBase easing, double startTime, double endTime, double s1, double s2) :
            base(easing, startTime, endTime, new[] { s1 }, new[] { s2 })
        {
        }

        public Scale(EasingFunctionBase easing, double startTime, double endTime, Span<double> start, Span<double> end)
            : base(easing, startTime, endTime, start, end)
        {
        }

        public Scale()
        {
        }
    }
}
