namespace Coosu.Storyboard.Events
{
    public sealed class Vector : CommonEvent
    {
        public override EventType EventType => EventTypes.Vector;

        public double StartScaleX
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public double StartScaleY
        {
            get => Start[1];
            set => Start[1] = value;
        }

        public double EndScaleX
        {
            get => End[0];
            set => End[0] = value;
        }

        public double EndScaleY
        {
            get => End[1];
            set => End[1] = value;
        }

        public Vector(EasingType easing, double startTime, double endTime, double vx1, double vy1, double vx2, double vy2) :
            base(easing, startTime, endTime, new[] { vx1, vy1 }, new[] { vx2, vy2 })
        {
        }

        public Vector()
        {
        }
    }
}
