using Coosu.Storyboard.Events;
using Coosu.Storyboard.Extensibility;

namespace Coosu.Storyboard.OsbX.ActionHandlers;

public class ColorActionHandler : BasicTimelineHandler<Color>
{
    public override int ParameterDimension => 3;
    public override string Flag => "C";
}