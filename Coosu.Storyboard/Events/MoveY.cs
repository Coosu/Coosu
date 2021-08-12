using Coosu.Storyboard.Common;
using Coosu.Storyboard.Management;

namespace Coosu.Storyboard.Events
{
    public sealed class MoveY : CommonEvent, IPositionAdjustable
    {
        public override EventType EventType => EventTypes.MoveY;

        public float StartY
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public float EndY
        {
            get => End[0];
            set => End[0] = value;
        }

        public MoveY(EasingType easing, float startTime, float endTime, float y1, float y2) :
            base(easing, startTime, endTime, new[] { y1 }, new[] { y2 })
        {
        }

        public MoveY()
        {
        }

        public void AdjustPosition(float x, float y)
        {
            Start[0] += y;
            End[0] += y;
        }
    }
}
