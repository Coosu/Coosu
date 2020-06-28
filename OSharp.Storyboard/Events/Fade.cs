namespace OSharp.Storyboard.Events
{
    public sealed class Fade : CommonEvent
    {
        public override EventType EventType => EventType.Fade;

        public float StartOpacity
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public float EndOpacity
        {
            get => End[0];
            set => End[0] = value;
        }

        public Fade(EasingType easing, float startTime, float endTime, float f1, float f2)
            : base(easing, startTime, endTime, new[] { f1 }, new[] { f2 })
        {
        }
    }
}
