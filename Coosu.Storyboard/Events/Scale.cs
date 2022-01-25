using System.Collections.Generic;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events
{
    public sealed class Scale : BasicEvent
    {
        public override EventType EventType => EventTypes.Scale;
        public override float DefaultValue { get; } = 1f;

        public float StartScale
        {
            get => GetValue(0);
            set => SetValue(0, value);
        }

        public float EndScale
        {
            get => GetValue(1);
            set => SetValue(1, value);
        }

        //public Scale(EasingFunctionBase easing, double startTime, double endTime, double s1, double s2) :
        //    base(easing, startTime, endTime, new[] { s1 }, new[] { s2 })
        //{
        //}

        public Scale(EasingFunctionBase easing, float startTime, float endTime, List<float> values)
            : base(easing, startTime, endTime, values)
        {
        }

        public Scale()
        {
        }
    }
}
