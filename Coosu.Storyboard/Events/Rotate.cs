namespace Coosu.Storyboard.Events
{
    public class Rotate : CommonEvent
    {
        public override EventType EventType => EventTypes.Rotate;

        public float StartRotate
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public float EndRotate
        {
            get => End[0];
            set => End[0] = value;
        }

        public Rotate(EasingType easing, float startTime, float endTime, float r1, float r2) :
            base(easing, startTime, endTime, new[] { r1 }, new[] { r2 })
        {
        }

        public Rotate()
        {
        }
    }
}
