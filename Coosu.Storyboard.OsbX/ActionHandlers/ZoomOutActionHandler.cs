using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.OsbX.Actions;

namespace Coosu.Storyboard.OsbX.ActionHandlers
{
    public class ZoomOutActionHandler : BasicTimelineHandler<ZoomOut>
    {
        public override string Flag => "ZO";
        public override int ParameterDimension => 1;
    }
}