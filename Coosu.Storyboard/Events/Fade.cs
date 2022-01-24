using System;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events
{
    public sealed class Fade : BasicEvent
    {
        public override EventType EventType => EventTypes.Fade;

        public double StartOpacity
        {
            get => ListValue[0];
            set => ListValue[0] = value;
        }

        public double EndOpacity
        {
            get => ListValue[1];
            set => ListValue[1] = value;
        }

        //public Fade(EasingFunctionBase easing, double startTime, double endTime, double f1, double f2)
        //    : base(easing, startTime, endTime, new[] { f1 }, new[] { f2 })
        //{
        //}

        public Fade(EasingFunctionBase easing, double startTime, double endTime, Span<double> start, Span<double> end)
            : base(easing, startTime, endTime, start, end)
        {
        }

        public Fade() { }
    }
}
