using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events
{
    public sealed class Fade : CommonEvent
    {
        public override EventType EventType => EventTypes.Fade;

        public double StartOpacity
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public double EndOpacity
        {
            get => End[0];
            set => End[0] = value;
        }

        public Fade(EasingType easing, double startTime, double endTime, double f1, double f2)
            : base(easing.ToEasingFunction(), startTime, endTime, new[] { f1 }, new[] { f2 })
        {
        }

        public Fade(IEasingFunction easing, double startTime, double endTime, double f1, double f2)
            : base(easing, startTime, endTime, new[] { f1 }, new[] { f2 })
        {
        }

        public Fade() { }
    }
}
