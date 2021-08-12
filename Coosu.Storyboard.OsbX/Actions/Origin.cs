using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.OsbX.Actions
{
    public class Origin : CommonEvent
    {
        public override EventType EventType => EventTypes.Scale;

        public double StartOrigin
        {
            get => Start[0];
            set => Start[0] = value;
        }

        public double EndOrigin
        {
            get => End[0];
            set => End[0] = value;
        }
    }
}