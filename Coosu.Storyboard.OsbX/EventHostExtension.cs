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
        else if (host is Camera25Object camera25Object)
        {
            if (basicEvent is Loop loop)
            {
                camera25Object.LoopList.Add(loop);
            }
            else
            {
                throw new NotSupportedException(
                    $"Child {basicEvent.GetType().FullName} is not supported for host {host.GetType().FullName}");
            }
        }
        else if (host is Sprite sprite)
        {
            if (basicEvent is Loop loop)
            {
                sprite.AddLoop(loop);
            }
            else if (basicEvent is Trigger trigger)
            {
                sprite.AddTrigger(trigger);
            }
            else
            {
                throw new NotSupportedException(
                    $"Child {basicEvent.GetType().FullName} is not supported for host {host.GetType().FullName}");
            }
        }
        else
        {
            throw new NotSupportedException(
                $"Child {basicEvent.GetType().FullName} is not supported for host {host.GetType().FullName}");
        }
    }
}