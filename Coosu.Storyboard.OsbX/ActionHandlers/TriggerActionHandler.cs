using Coosu.Shared;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Extensibility;

namespace Coosu.Storyboard.OsbX.ActionHandlers;

public class TriggerActionHandler : ActionHandler<Trigger>
{
    public override string Flag => "T";

    public override Trigger Deserialize(ref ValueListBuilder<string> split)
    {
        var triggerName = split[1];
        var startTime = double.Parse(split[2]);
        var endTime = int.Parse(split[3]);
        return new Trigger(startTime, endTime, triggerName);
    }

    public override string Serialize(Trigger trigger)
    {
        return $"T,{trigger.TriggerName},{trigger.StartTime},{trigger.EndTime}";
    }
}