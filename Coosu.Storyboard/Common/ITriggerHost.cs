using System.Collections.Generic;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Common;

public interface ITriggerHost
{
    IReadOnlyList<Trigger> TriggerList { get; }
    void AddTrigger(Trigger trigger);
    bool RemoveTrigger(Trigger trigger);
    void ClearTriggers();
}