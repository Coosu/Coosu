using System;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events
{
    public sealed class Move : BasicEvent, IPositionAdjustable
    {
        public override EventType EventType => EventTypes.Move;

        public double StartX
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public double StartY
        {
            get => Start[1];
            set => Start[1] = value;
        }

        public double EndX
        {
            get => End[0];
            set => End[0] = value;
        }

        public double EndY
        {
            get => End[1];
            set => End[1] = value;
        }

        public Move(EasingFunctionBase easing, double startTime, double endTime, double x1, double y1, double x2, double y2) :
            base(easing, startTime, endTime, new[] { x1, y1 }, new[] { x2, y2 })
        {

        }

        public Move(EasingFunctionBase easing, double startTime, double endTime, Span<double> start, Span<double> end)
            : base(easing, startTime, endTime, start, end)
        {
        }

        public Move()
        {
        }

        public void AdjustPosition(double x, double y)
        {
            Start[0] += x;
            Start[1] += y;
            End[0] += x;
            End[1] += y;
        }
    }
}
