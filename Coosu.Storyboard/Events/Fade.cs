using System.Collections.Generic;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events
{
    public sealed class Fade : BasicEvent
    {
        public override EventType EventType => EventTypes.Fade;
        public override float DefaultValue { get; } = 1f;

        public float StartOpacity
        {
            get => GetValue(0);
            set => SetValue(0, value);
        }

        public float EndOpacity
        {
            get => GetValue(1);
            set => SetValue(1, value);
        }

        //public Fade(EasingFunctionBase easing, double startTime, double endTime, double f1, double f2)
        //    : base(easing, startTime, endTime, new[] { f1 }, new[] { f2 })
        //{
        //}

        public Fade(EasingFunctionBase easing, float startTime, float endTime, List<float> values)
            : base(easing, startTime, endTime, values)
        {
        }

        public Fade() { }
    }
}
