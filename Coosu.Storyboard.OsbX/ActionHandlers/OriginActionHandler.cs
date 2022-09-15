using Coosu.Storyboard.Extensibility;

namespace Coosu.Storyboard.OsbX.ActionHandlers;

public class OriginActionHandler : BasicTimelineHandler<Actions.Origin>
{
    public override int ParameterDimension => 2;
    public override string Flag => "O";
}