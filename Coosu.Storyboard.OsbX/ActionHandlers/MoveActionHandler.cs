using Coosu.Storyboard.Events;
using Coosu.Storyboard.Extensibility;

namespace Coosu.Storyboard.OsbX.ActionHandlers
{
    public class MoveActionHandler : BasicTimelineHandler<Move>
    {
        public override int ParameterDimension => 2;
        public override string Flag => "M";
    }
}