using System;
using System.Linq;
using Coosu.Shared.Mathematics;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Management
{
    public static class EventCompare
    {

        public static bool InObsoleteTimingRange(this CommonEvent e, EventHost host, out RangeValue<float> range)
        {
            return host.ObsoleteList.ContainsTimingPoint(out range, e.StartTime, e.EndTime);
        }

        public static bool OnObsoleteTimingRange(this CommonEvent e, EventHost host)
        {
            return host.ObsoleteList.OnTimingRange(out _, e.StartTime) ||
                   host.ObsoleteList.OnTimingRange(out _, e.EndTime);
        }

        public static bool IsEventSequent(CommonEvent previous, CommonEvent next)
        {
            return previous.End.SequenceEqual(next.Start);
        }

        public static bool EndsWithUnworthy(this CommonEvent e)
        {
            return EventExtension.UnworthyDictionary.ContainsKey(e.EventType) &&
                   EventExtension.UnworthyDictionary[e.EventType].SequenceEqual(e.End);
        }

        public static bool IsStaticAndDefault(this CommonEvent e)
        {
            return e.IsDefault() &&
                   e.IsStatic();
        }

        public static bool IsDefault(this CommonEvent e)
        {
            return EventExtension.DefaultDictionary.ContainsKey(e.EventType) &&
                   e.Start.SequenceEqual(EventExtension.DefaultDictionary[e.EventType]);
        }

        public static bool IsStatic(this CommonEvent e)
        {
            return e.Start.SequenceEqual(e.End);
        }

        public static bool EqualsInitialPosition(this Move move, Sprite sprite)
        {
            return move.StartX.Equals(sprite.DefaultX) &&
                   move.StartY.Equals(sprite.DefaultY);
        }

        public static bool IsTimeInRange(this CommonEvent e, EventHost host)
        {
            return e.IsSmallerThenMaxTime(host) && e.IsLargerThanMinTime(host);
        }

        public static bool IsSmallerThenMaxTime(this CommonEvent e, EventHost host)
        {
            return e.EndTime < host.MaxTime ||
                   e.EqualsMultiMaxTime(host);
        }

        public static bool IsLargerThanMinTime(this CommonEvent e, EventHost host)
        {
            return e.StartTime > host.MinTime ||
                   e.EqualsMultiMinTime(host);
        }

        public static bool EqualsMultiMaxTime(this CommonEvent e, EventHost host)
        {
            return e.EqualsMaxTime(host) && host.GetMaxTimeCount() > 1;
        }

        public static bool EqualsMultiMinTime(this CommonEvent e, EventHost host)
        {
            return e.EqualsMinTime(host) && host.GetMinTimeCount() > 1;
        }

        public static bool EqualsMaxTime(this CommonEvent e, EventHost host)
        {
            return e.EndTime == host.MaxTime;
        }

        public static bool EqualsMinTime(this CommonEvent e, EventHost host)
        {
            return e.StartTime == host.MinTime;
        }
    }
}