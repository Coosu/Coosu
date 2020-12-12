using Coosu.Storyboard.Events.Extension;

namespace Coosu.Osbx.ActionHandlers
{
    public class ZoomInActionHandler : BasicTimelineHandler<ZoomIn>
    {
        public override int ParameterDimension => 1;
        public override string Flag => "ZI";
    }
}