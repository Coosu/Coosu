using OSharp.Shared.Mathematics;
using OSharp.Storyboard.Events;
using OSharp.Storyboard.Internal;
using System.Linq;

namespace OSharp.Storyboard.Management
{
    public static class EventCompare
    {

        public static bool InObsoleteTimingRange(this CommonEvent e, EventContainer container, out RangeValue<float> range)
        {
            return container.ObsoleteList.ContainsTimingPoint(out range, e.StartTime, e.EndTime);
        }

        public static bool OnObsoleteTimingRange(this CommonEvent e, EventContainer container)
        {
            return container.ObsoleteList.OnTimingRange(out _, e.StartTime) ||
                   container.ObsoleteList.OnTimingRange(out _, e.EndTime);
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

        public static bool EqualsInitialPosition(this Move move, Element element)
        {
            return move.StartX.Equals(element.DefaultX) &&
                   move.StartY.Equals(element.DefaultY);
        }

        public static bool IsTimeInRange(this CommonEvent e, EventContainer container)
        {
            return e.IsSmallerThenMaxTime(container) && e.IsLargerThanMinTime(container);
        }

        public static bool IsSmallerThenMaxTime(this CommonEvent e, EventContainer container)
        {
            return e.EndTime < container.MaxTime ||
                   e.EqualsMultiMaxTime(container);
        }

        public static bool IsLargerThanMinTime(this CommonEvent e, EventContainer container)
        {
            return e.StartTime > container.MinTime ||
                   e.EqualsMultiMinTime(container);
        }

        public static bool EqualsMultiMaxTime(this CommonEvent e, EventContainer container)
        {
            return e.EqualsMaxTime(container) && container.MaxTimeCount > 1;
        }

        public static bool EqualsMultiMinTime(this CommonEvent e, EventContainer container)
        {
            return e.EqualsMinTime(container) && container.MinTimeCount > 1;
        }

        public static bool EqualsMaxTime(this CommonEvent e, EventContainer container)
        {
            return e.EndTime == container.MaxTime;
        }

        public static bool EqualsMinTime(this CommonEvent e, EventContainer container)
        {
            return e.StartTime == container.MinTime;
        }
    }
}