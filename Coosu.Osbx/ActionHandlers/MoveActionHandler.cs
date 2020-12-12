using Coosu.Storyboard.Events;

namespace Coosu.Osbx.ActionHandlers
{
    public class MoveActionHandler : BasicTimelineHandler<Move>
    {
        public override int ParameterDimension => 2;
        public override string Flag => "M";
    }
}