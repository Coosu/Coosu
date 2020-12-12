namespace Coosu.Storyboard.Events
{
    public sealed class Move : CommonEvent, IAdjustablePositionEvent
    {
        public override EventType EventType => EventTypes.Move;

        public float StartX
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public float StartY
        {
            get => Start[1];
            set => Start[1] = value;
        }

        public float EndX
        {
            get => End[0];
            set => End[0] = value;
        }

        public float EndY
        {
            get => End[1];
            set => End[1] = value;
        }

        public Move(EasingType easing, float startTime, float endTime, float x1, float y1, float x2, float y2) :
            base(easing, startTime, endTime, new[] { x1, y1 }, new[] { x2, y2 })
        {

        }

        public Move()
        {
        }

        public void AdjustPosition(float x, float y)
        {
            Start[0] += x;
            Start[1] += y;
            End[0] += x;
            End[1] += y;
        }
    }
}
