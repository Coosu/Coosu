using System;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events
{
    public sealed class Fade : BasicEvent
    {
        public override EventType EventType => EventTypes.Fade;

        public double StartOpacity
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public double EndOpacity
        {
            get => End[0];
            set => End[0] = value;
        }

        public Fade(EasingFunctionBase easing, double startTime, double endTime, double f1, double f2)
            : base(easing, startTime, endTime, new[] { f1 }, new[] { f2 })
        {
        }

        public Fade(EasingFunctionBase easing, double startTime, double endTime, Span<double> start, Span<double> end)
            : base(easing, startTime, endTime, start, end)
        {
        }

        public Fade() { }
    }
}
