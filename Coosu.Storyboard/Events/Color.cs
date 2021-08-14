namespace Coosu.Storyboard.Events
{
    public sealed class Color : CommonEvent
    {
        public override EventType EventType => EventTypes.Color;

        public double StartR
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public double StartG
        {
            get => Start[1];
            set => Start[1] = value;
        }

        public double StartB
        {
            get => Start[2];
            set => Start[2] = value;
        }

        public double EndR
        {
            get => End[0];
            set => End[0] = value;
        }

        public double EndG
        {
            get => End[1];
            set => End[1] = value;
        }

        public double EndB
        {
            get => End[2];
            set => End[2] = value;
        }

        public Color(EasingType easing, double startTime, double endTime, double r1, double g1, double b1, double r2,
            double g2, double b2) : base(easing, startTime, endTime, new[] { r1, g1, b1 }, new[] { r2, g2, b2 })
        {
        }
    }
}
