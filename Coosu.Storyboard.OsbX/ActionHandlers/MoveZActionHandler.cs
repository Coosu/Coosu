using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.OsbX.Actions;

namespace Coosu.Storyboard.OsbX.ActionHandlers;

public class MoveZActionHandler : BasicTimelineHandler<MoveZ>
{
    public override int ParameterDimension => 1;
    public override string Flag => "MZ";
}