using System;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.OsbX;

public static class EventHostExtension
{
    public static void ApplyAction(this IDetailedEventHost host, IKeyEvent basicEvent)
    {
        host.AddEvent(basicEvent);
    }

    public static void ApplyAction<T>(this IDetailedEventHost host, T basicEvent) where T : IEvent
    {
        if (basicEvent is IKeyEvent keyEvent)
        {
            host.AddEvent(keyEvent);
        }
        else if (host is ILoopHost loopHost && basicEvent is Loop loop)
        {
            loopHost.AddLoop(loop);
        }
        else if (host is ITriggerHost triggerHost && basicEvent is Trigger trigger)
        {
            triggerHost.AddTrigger(trigger);
        }
        else
        {
            throw new NotSupportedException(
                $"Child {basicEvent.GetType().FullName} is not supported for host {host.GetType().FullName}");
        }
    }
}