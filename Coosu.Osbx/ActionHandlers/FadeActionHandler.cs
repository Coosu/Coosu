using Coosu.Storyboard.Events;

namespace Coosu.Osbx.ActionHandlers
{
    public class FadeActionHandler : BasicTimelineHandler<Fade>
    {
        public override int ParameterDimension => 1;
        public override string Flag => "F";
    }
}