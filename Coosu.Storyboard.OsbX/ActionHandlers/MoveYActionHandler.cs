using Coosu.Storyboard.Events;
using Coosu.Storyboard.Extensibility;

namespace Coosu.Storyboard.OsbX.ActionHandlers;

public class MoveYActionHandler : BasicTimelineHandler<MoveY>
{
    public override int ParameterDimension => 1;
    public override string Flag => "MY";
}