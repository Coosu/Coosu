using Coosu.Osbx.Actions;
using Coosu.Storyboard.Extensibility;

namespace Coosu.Osbx.ActionHandlers
{
    public class ZoomOutActionHandler : BasicTimelineHandler<ZoomOut>
    {
        public override string Flag => "ZO";
        public override int ParameterDimension => 1;
    }
}