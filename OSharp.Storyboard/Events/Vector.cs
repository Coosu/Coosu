namespace OSharp.Storyboard.Events
{
    public sealed class Vector : CommonEvent
    {
        public override EventType EventType => EventType.Vector;

        public float StartScaleX
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public float StartScaleY
        {
            get => Start[1];
            set => Start[1] = value;
        }

        public float EndScaleX
        {
            get => End[0];
            set => End[0] = value;
        }

        public float EndScaleY
        {
            get => End[1];
            set => End[1] = value;
        }

        public Vector(EasingType easing, float startTime, float endTime, float vx1, float vy1, float vx2, float vy2) :
            base(easing, startTime, endTime, new[] { vx1, vy1 }, new[] { vx2, vy2 })
        {
        }
    }
}
