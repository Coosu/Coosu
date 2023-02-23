using Coosu.Shared;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Extensibility;

namespace Coosu.Storyboard.OsbX.ActionHandlers;

public class LoopActionHandler : ActionHandler<Loop>
{
    public override string Flag => "L";
    public override int ParameterDimension { get; } = 0;

    public override Loop Deserialize(ref ValueListBuilder<string> split)
    {
        var startTime = double.Parse(split[1]);
        var loopTimes = int.Parse(split[2]);
        return new Loop(startTime, loopTimes);
    }

    public override string Serialize(Loop loop)
    {
        return $"L,{loop.StartTime},{loop.LoopCount}";
    }
}