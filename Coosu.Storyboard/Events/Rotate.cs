using System.Collections.Generic;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events
{
    public class Rotate : BasicEvent
    {
        public override EventType EventType => EventTypes.Rotate;

        public float StartRotate
        {
            get => GetValue(0);
            set => SetValue(0, value);
        }

        public float EndRotate
        {
            get => GetValue(1);
            set => SetValue(1, value);
        }

        //public Rotate(EasingFunctionBase easing, double startTime, double endTime, double r1, double r2) :
        //    base(easing, startTime, endTime, new[] { r1 }, new[] { r2 })
        //{
        //}

        public Rotate(EasingFunctionBase easing, float startTime, float endTime, List<float> values)
            : base(easing, startTime, endTime, values)
        {
        }

        public Rotate()
        {
        }
    }
}
