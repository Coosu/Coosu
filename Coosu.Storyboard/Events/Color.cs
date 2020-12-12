namespace Coosu.Storyboard.Events
{
    public sealed class Color : CommonEvent
    {
        public override EventType EventType => EventTypes.Color;

        public float StartR
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public float StartG
        {
            get => Start[1];
            set => Start[1] = value;
        }

        public float StartB
        {
            get => Start[2];
            set => Start[2] = value;
        }

        public float EndR
        {
            get => End[0];
            set => End[0] = value;
        }

        public float EndG
        {
            get => End[1];
            set => End[1] = value;
        }

        public float EndB
        {
            get => End[2];
            set => End[2] = value;
        }

        public Color(EasingType easing, float startTime, float endTime, float r1, float g1, float b1, float r2,
            float g2, float b2) : base(easing, startTime, endTime, new[] { r1, g1, b1 }, new[] { r2, g2, b2 })
        {
        }
    }
}
