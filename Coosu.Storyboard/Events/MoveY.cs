using System.Collections.Generic;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events
{
    public sealed class MoveY : BasicEvent, IPositionAdjustable
    {
        public override EventType EventType => EventTypes.MoveY;

        public double StartY
        {
            get => GetValue(0);
            set => SetValue(0, value);
        }

        public double EndY
        {
            get => GetValue(1);
            set => SetValue(1, value);
        }

        //public MoveY(EasingFunctionBase easing, double startTime, double endTime, double y1, double y2) :
        //    base(easing, startTime, endTime, new[] { y1 }, new[] { y2 })
        //{
        //}

        public MoveY(EasingFunctionBase easing, double startTime, double endTime, List<double> values)
            : base(easing, startTime, endTime, values)
        {
        }

        public MoveY()
        {
        }

        public void AdjustPosition(double x, double y)
        {
            StartY += y;
            EndY += y;
        }
    }
}
