using System.Linq;
using Coosu.Storyboard.Common;

// ReSharper disable once CheckNamespace
namespace Coosu.Storyboard;

public static class EventHostExtensions
{
    public static void Adjust(this IDetailedEventHost eventHost, double offsetX, double offsetY, double offsetTiming)
    {
        if (eventHost is ISceneObject iso)
        {
            iso.DefaultX += offsetX;
            iso.DefaultY += offsetY;

            if (iso is ILoopHost loopHost)
            {
                foreach (var loop in loopHost.LoopList)
                    loop.Adjust(offsetX, offsetY, offsetTiming);
            }

            if (iso is ITriggerHost triggerHost)
            {
                foreach (var trigger in triggerHost.TriggerList)
                    trigger.Adjust(offsetX, offsetY, offsetTiming);
            }
        }

        var events = eventHost.Events.GroupBy(k => k.EventType);
        foreach (var kv in events)
        {
            foreach (var e in kv)
            {
                if (e is IPositionAdjustable adjustable)
                    adjustable.AdjustPosition(offsetX, offsetY);

                e.AdjustTiming(offsetTiming);
            }
        }
    }
}