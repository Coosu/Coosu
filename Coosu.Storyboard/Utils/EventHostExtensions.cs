using System;
using System.Collections.Generic;
using System.Linq;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Utils
{
    public static class EventHostExtensions
    {
        public static void Adjust(this IDetailedEventHost eventHost, double offsetX, double offsetY, double offsetTiming)
        {
            if (eventHost is ISceneObject iso)
            {
                iso.DefaultX += offsetX;
                iso.DefaultY += offsetY;

                if (iso is Sprite sprite)
                {
                    foreach (var loop in sprite.LoopList)
                        loop.Adjust(offsetX, offsetY, offsetTiming);

                    foreach (var trigger in sprite.TriggerList)
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

        public static int GetMaxTimeCount(this ISceneObject eventHost)
        {
            var maxTime = eventHost.MaxTime;
            if (eventHost is Sprite sprite)
            {
                return sprite.Events.Count(k => k.EndTime.Equals(maxTime)) +
                       sprite.LoopList.Count(k => k.OuterMaxTime.Equals(maxTime)) +
                       sprite.TriggerList.Count(k => k.MaxTime.Equals(maxTime));
            }

            return eventHost.Events.Count(k => k.EndTime.Equals(maxTime));
        }

        public static int GetMinTimeCount(this ISceneObject eventHost)
        {
            var minTime = eventHost.MinTime;
            if (eventHost is Sprite sprite)
            {
                return sprite.Events.Count(k => k.StartTime.Equals(minTime)) +
                       sprite.LoopList.Count(k => k.OuterMinTime.Equals(minTime)) +
                       sprite.TriggerList.Count(k => k.MinTime.Equals(minTime));
            }

            return eventHost.Events.Count(k => k.StartTime.Equals(minTime));
        }

        public static int GetMaxTimeCount(this IDetailedEventHost eventHost)
        {
            return eventHost.Events.Count(k => k.EndTime.Equals(eventHost.MaxTime));
        }

        public static int GetMinTimeCount(this IDetailedEventHost eventHost)
        {
            return eventHost.Events.Count(k => k.StartTime.Equals(eventHost.MinTime));
        }

        public static bool HasEffectiveTiming(this IDetailedEventHost eventHost)
        {
            if (eventHost.MaxTime < eventHost.MinTime)
                return false;
            if (eventHost.MaxTime.Equals(eventHost.MinTime))
                return false;
            return true;
        }
    }
}