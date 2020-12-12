using Coosu.Storyboard.Events;

namespace Coosu.Osbx.ActionHandlers
{
    public class VectorActionHandler : BasicTimelineHandler<Vector>
    {
        public override int ParameterDimension => 2;
        public override string Flag => "V";
    }
}