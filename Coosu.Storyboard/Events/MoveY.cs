using System;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events
{
    public sealed class MoveY : BasicEvent, IPositionAdjustable
    {
        public override EventType EventType => EventTypes.MoveY;

        public double StartY
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public double EndY
        {
            get => End[0];
            set => End[0] = value;
        }

        public MoveY(EasingFunctionBase easing, double startTime, double endTime, double y1, double y2) :
            base(easing, startTime, endTime, new[] { y1 }, new[] { y2 })
        {
        }

        public MoveY(EasingFunctionBase easing, double startTime, double endTime, Span<double> start, Span<double> end)
            : base(easing, startTime, endTime, start, end)
        {
        }

        public MoveY()
        {
        }

        public void AdjustPosition(double x, double y)
        {
            Start[0] += y;
            End[0] += y;
        }
    }
}
