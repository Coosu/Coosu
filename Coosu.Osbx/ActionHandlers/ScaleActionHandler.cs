using Coosu.Storyboard.Events;
using Coosu.Storyboard.Parsing;

namespace Coosu.Osbx.ActionHandlers
{
    public class ScaleActionHandler : BasicTimelineHandler<Scale>
    {
        public override int ParameterDimension => 1;
        public override string Flag => "S";
    }
}