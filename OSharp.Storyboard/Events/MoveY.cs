﻿namespace OSharp.Storyboard.Events
{
    public sealed class MoveY : CommonEvent, IAdjustablePositionEvent
    {
        public override EventType EventType => EventType.MoveY;

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

        public void AdjustPosition(float x, float y)
        {
            Start[0] += y;
            End[0] += y;
        }
    }
}
