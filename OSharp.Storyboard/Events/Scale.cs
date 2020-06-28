namespace OSharp.Storyboard.Events
{
    public sealed class Scale : CommonEvent
    {
        public override EventType EventType => EventType.Scale; 
       
        public float StartScale
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public float EndScale
        {
            get => End[0];
            set => End[0] = value;
        }

        public Scale(EasingType easing, float startTime, float endTime, float s1, float s2):
            base(easing, startTime, endTime, new[] { s1 }, new[] { s2 })
        {
        }
    }
}
