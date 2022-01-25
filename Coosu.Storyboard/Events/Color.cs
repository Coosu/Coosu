using System.Collections.Generic;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events
{
    public sealed class Color : BasicEvent
    {
        public override EventType EventType => EventTypes.Color;

        public float StartR
        {
            get => GetValue(0);
            set => SetValue(0, value);
        }

        public float StartG
        {
            get => GetValue(1);
            set => SetValue(1, value);
        }

        public float StartB
        {
            get => GetValue(2);
            set => SetValue(2, value);
        }

        public float EndR
        {
            get => GetValue(3);
            set => SetValue(3, value);
        }

        public float EndG
        {
            get => GetValue(4);
            set => SetValue(4, value);
        }

        public float EndB
        {
            get => GetValue(5);
            set => SetValue(5, value);
        }

        //public Color(EasingFunctionBase easing, double startTime, double endTime,
        //    double r1, double g1, double b1, double r2, double g2, double b2)
        //    : base(easing, startTime, endTime, new[] { r1, g1, b1 }, new[] { r2, g2, b2 })
        //{
        //}

        public Color(EasingFunctionBase easing, float startTime, float endTime, List<float> values)
            : base(easing, startTime, endTime, values)
        {
        }

        public Color()
        {
        }
    }
}
