using System.Collections.Generic;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events
{
    public sealed class MoveX : BasicEvent, IPositionAdjustable
    {
        public override EventType EventType => EventTypes.MoveX;

        public float StartX
        {
            get => GetValue(0);
            set => SetValue(0, value);
        }

        public float EndX
        {
            get => GetValue(1);
            set => SetValue(1, value);
        }

        //public MoveX(EasingFunctionBase easing, double startTime, double endTime, double x1, double x2) :
        //    base(easing, startTime, endTime, new[] { x1 }, new[] { x2 })
        //{
        //}

        public MoveX(EasingFunctionBase easing, float startTime, float endTime, List<float> values)
            : base(easing, startTime, endTime, values)
        {
        }

        public MoveX()
        {
        }

        public void AdjustPosition(float x, float y)
        {
            StartX += x;
            EndX += x;
        }
    }
}
