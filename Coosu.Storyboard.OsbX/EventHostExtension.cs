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
        else if (host is Sprite sprite && basicEvent is Loop loop)
        {
            sprite.AddLoop(loop);
        }
        else
        {
            throw new NotSupportedException(
                $"Child {basicEvent.GetType().FullName} is not supported for host {host.GetType().FullName}");
        }
    }
}