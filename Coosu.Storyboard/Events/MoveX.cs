using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;

namespace Coosu.Storyboard.Events
{
    public sealed class MoveX : CommonEvent, IPositionAdjustable
    {
        public override EventType EventType => EventTypes.MoveX;

        public double StartX
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public double EndX
        {
            get => End[0];
            set => End[0] = value;
        }

        public MoveX(EasingType easing, double startTime, double endTime, double x1, double x2) :
            base(easing.ToEasingFunction(), startTime, endTime, new[] { x1 }, new[] { x2 })
        {
        }

        public MoveX(IEasingFunction easing, double startTime, double endTime, double x1, double x2) :
            base(easing, startTime, endTime, new[] { x1 }, new[] { x2 })
        {
        }

        public MoveX()
        {
        }

        public void AdjustPosition(double x, double y)
        {
            Start[0] += x;
            End[0] += x;
        }
    }
}
