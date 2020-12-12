using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.OsbX.Actions;

namespace Coosu.Storyboard.OsbX.ActionHandlers
{
    public class ZoomInActionHandler : BasicTimelineHandler<ZoomIn>
    {
        public override int ParameterDimension => 1;
        public override string Flag => "ZI";
    }
}