using Coosu.Storyboard.Events;
using Coosu.Storyboard.Extensibility;

namespace Coosu.Storyboard.OsbX.ActionHandlers;

public class FadeActionHandler : BasicTimelineHandler<Fade>
{
    public override int ParameterDimension => 1;
    public override string Flag => "F";
}