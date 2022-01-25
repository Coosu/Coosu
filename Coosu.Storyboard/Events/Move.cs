using System.Collections.Generic;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events
{
    public sealed class Move : BasicEvent, IPositionAdjustable
    {
        public override EventType EventType => EventTypes.Move;

        public float StartX
        {
            get => GetValue(0);
            set => SetValue(0, value);
        }

        public float StartY
        {
            get => GetValue(1);
            set => SetValue(1, value);
        }

        public float EndX
        {
            get => GetValue(2);
            set => SetValue(2, value);
        }

        public float EndY
        {
            get => GetValue(3);
            set => SetValue(3, value);
        }

        //public Move(EasingFunctionBase easing, double startTime, double endTime, double x1, double y1, double x2, double y2) :
        //    base(easing, startTime, endTime, new[] { x1, y1 }, new[] { x2, y2 })
        //{

        //}

        public Move(EasingFunctionBase easing, float startTime, float endTime, List<float> values)
            : base(easing, startTime, endTime, values)
        {
        }

        public Move()
        {
        }

        public void AdjustPosition(float x, float y)
        {
            StartX += x;
            StartY += y;
            EndX += x;
            EndY += y;
        }
    }
}
