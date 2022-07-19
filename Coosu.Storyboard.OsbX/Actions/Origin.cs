using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.OsbX.Actions
{
    public class Origin : BasicEvent
    {
        public override EventType EventType => EventTypes.Scale;

        public double StartOrigin
        {
            get => GetValue(0);
            set => SetValue(0, value);
        }

        public double EndOrigin
        {
            get => GetValue(1);
            set => SetValue(1, value);
        }
    }
}