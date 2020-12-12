namespace Coosu.Storyboard.Events
{
    public sealed class MoveX : CommonEvent, IAdjustablePositionEvent
    {
        public override EventType EventType => EventTypes.MoveX;

        public float StartX
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public float EndX
        {
            get => End[0];
            set => End[0] = value;
        }

        public MoveX(EasingType easing, float startTime, float endTime, float x1, float x2) :
            base(easing, startTime, endTime, new[] { x1 }, new[] { x2 })
        {
        }

        public MoveX()
        {
        }

        public void AdjustPosition(float x, float y)
        {
            Start[0] += x;
            End[0] += x;
        }
    }
}
