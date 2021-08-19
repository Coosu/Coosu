using System.Linq;
using Coosu.Shared.Mathematics;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Extensions.Optimizing
{
    public static class EventCompare
    {

        public static bool InObsoleteTimingRange(this ICommonEvent e, TimeRange obsoleteList, out RangeValue<double> range)
        {
            return obsoleteList.ContainsTimingPoint(out range, e.StartTime, e.EndTime);
        }

        public static bool OnObsoleteTimingRange(this ICommonEvent e, TimeRange obsoleteList)
        {
            return obsoleteList.OnTimingRange(out _, e.StartTime) ||
                   obsoleteList.OnTimingRange(out _, e.EndTime);
        }

        public static bool IsEventSequent(CommonEvent previous, CommonEvent next)
        {
            return previous.End.SequenceEqual(next.Start);
        }

        public static bool EndsWithIneffective(this ICommonEvent e)
        {
            return EventExtensions.IneffectiveDictionary.ContainsKey(e.EventType) &&
                   EventExtensions.IneffectiveDictionary[e.EventType].SequenceEqual(e.End);
        }

        public static bool IsStaticAndDefault(this ICommonEvent e)
        {
            return e.IsDefault() &&
                   e.IsStatic();
        }

        public static bool IsStatic(this ICommonEvent e)
        {
            return e.Start.SequenceEqual(e.End);
        }

        public static bool EqualsInitialPosition(this Move move, Sprite sprite)
        {
            return move.StartX.Equals(sprite.DefaultX) &&
                   move.StartY.Equals(sprite.DefaultY);
        }

        public static bool IsTimeInRange(this ICommonEvent e, IEventHost host)
        {
            return e.IsSmallerThenMaxTime(host) && e.IsLargerThanMinTime(host);
        }

        public static bool IsSmallerThenMaxTime(this ICommonEvent e, IEventHost host)
        {
            return e.EndTime < host.MaxTime ||
                   e.EqualsMultiMaxTime(host);
        }

        public static bool IsLargerThanMinTime(this ICommonEvent e, IEventHost host)
        {
            return e.StartTime > host.MinTime ||
                   e.EqualsMultiMinTime(host);
        }

        public static bool EqualsMultiMaxTime(this ICommonEvent e, IEventHost host)
        {
            return e.EqualsMaxTime(host) && host.GetMaxTimeCount() > 1;
        }

        public static bool EqualsMultiMinTime(this ICommonEvent e, IEventHost host)
        {
            return e.EqualsMinTime(host) && host.GetMinTimeCount() > 1;
        }

        public static bool EqualsMaxTime(this ICommonEvent e, IEventHost host)
        {
            return e.EndTime == host.MaxTime;
        }

        public static bool EqualsMinTime(this ICommonEvent e, IEventHost host)
        {
            return e.StartTime == host.MinTime;
        }
    }
}