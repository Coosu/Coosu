using Coosu.Storyboard.Events;
using Coosu.Storyboard.Parsing;

namespace Coosu.Osbx.ActionHandlers
{
    public class MoveActionHandler : BasicTimelineHandler<Move>
    {
        public override int ParameterDimension => 2;
        public override string Flag => "M";
    }
}