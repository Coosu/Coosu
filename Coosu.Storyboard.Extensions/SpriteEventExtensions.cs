using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Extensions.Optimizing;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Extensions
{
    public static class SpriteEventExtensions
    {
        /// <summary>
        /// Standardize an <see cref="IEventHost"/>'s events to osu!storyboard standard events.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="discretizingInterval">Discretizing interval in milliseconds.
        /// The value should be larger than 16 as recommended in osu!wiki.</param>
        /// <param name="discretizingAccuracy">Discretizing accuracy that how many numbers will be preserved after decimal point.
        /// If the value is null the numbers will not be rounded.</param>
        /// <returns></returns>
        public static void StandardizeEvents(this IEventHost host,
            int discretizingInterval,
            int? discretizingAccuracy)
        {
            DiscretizeNonStandardEasing(host, discretizingInterval, discretizingAccuracy);
            ComputeRelativeEvents(host, discretizingInterval, discretizingAccuracy);
        }

        /// <summary>
        /// Make non-standard easing discretized.
        /// </summary>
        private static void DiscretizeNonStandardEasing(IEventHost sprite,
            int discretizingInterval,
            int? discretizingAccuracy)
        {
            var commonEvents = sprite.Events.ToList();
            foreach (var @event in commonEvents
                .Where(k => k is not RelativeEvent && k.Easing.TryGetEasingType() == null))
            {
                var eventList = @event.ComputeDiscretizedEvents(false,
                    discretizingInterval,
                    discretizingAccuracy);
                sprite.Events.Remove(@event);
                foreach (var commonEvent in eventList)
                {
                    sprite.Events.Add(commonEvent);
                }
            }
        }

        private static void ComputeRelativeEvents(IEventHost sprite,
            int discretizingInterval,
            int? discretizingAccuracy)
        {
            var commonEvents = sprite.Events.ToList();
            foreach (var grouping in commonEvents
                    .Where(k => k is RelativeEvent)
                    .Cast<RelativeEvent>().GroupBy(k => k.EventType))
            // todo: discretize and compute all at once to improve performance
            {
                var targetStdType = EventTypes.GetValue(grouping.Key.Index - 100);
                int j = 0;
                foreach (var @event in grouping)
                {
                    j++;
                    try
                    {
                        Console.WriteLine();
                        Console.WriteLine("Step " + j + ": " + @event.GetHeaderString());
                        var allThisTypeStandardEvents = sprite
                            .Events
                            .Where(k => k is CommonEvent &&
                                        k.Easing.TryGetEasingType() != null &&
                                        k.EventType == targetStdType)
                            .OrderBy(k => k.StartTime)
                            .Cast<CommonEvent>()
                            .ToList();

                        var startTime = (int)@event.StartTime;
                        var endTime = (int)@event.EndTime;

                        var targetStandardEvents = allThisTypeStandardEvents
                            .Where(k => k.StartTime < endTime && k.EndTime > startTime) // k.EndTime > startTime
                            .ToList();
                        if (allThisTypeStandardEvents.Count == 0 && sprite is ICameraUsable camerable)
                        {
                            var defaultValue = targetStdType.GetDefaultValue(camerable) ?? throw new NotSupportedException(
                                targetStdType.Flag + " doesn't have any default value.");
                            var newEvent = CommonEvent.Create(targetStdType, @event.Easing, startTime, endTime,
                                defaultValue,
                                @event.EventType.ComputeRelative(defaultValue, @event.End));
                            AddEventDirectly(sprite, newEvent, discretizingInterval, discretizingAccuracy);
                            sprite.Events.Remove(@event);
                        }
                        else if (targetStandardEvents.Count == 0)
                        {
                            var lastValue = sprite.ComputeFrame(targetStdType, startTime, 0);
                            var newEvent = CommonEvent.Create(targetStdType, @event.Easing, startTime, endTime,
                                lastValue,
                                @event.EventType.ComputeRelative(lastValue, @event.End));
                            AddEventDirectly(sprite, newEvent, discretizingInterval, discretizingAccuracy);
                            sprite.Events.Remove(@event);

                            var nextEvents = allThisTypeStandardEvents
                                .Where(k => k.StartTime >= endTime)
                                .ToList();
                            foreach (var commonEvent in nextEvents)
                            {
                                for (int i = 0; i < commonEvent.EventType.Size; i++)
                                {
                                    commonEvent.Start[i] += @event.End[i]; // offset
                                    commonEvent.End[i] += @event.End[i];
                                }
                            }
                        }
                        else
                        {
                            var discretizedRelatives = @event.ComputeDiscretizedEvents(false,
                                    discretizingInterval,
                                    null)
                                .Where(k => !k.Start.SequenceEqual(k.End))
                                .ToDictionary(k => (k.StartTime, k.EndTime), k => k);
                            endTime = (int)discretizedRelatives.Last().Key.EndTime;
                            var discretizingTargetStandardEvents =
                                new Dictionary<(double StartTime, double EndTime), ICommonEvent>();
                            //var boundedDiscretizingTargetStandardEvents = new HashSet<ICommonEvent>(); //todo: 算上因特殊情况被再度切割的？
                            foreach (CommonEvent k in targetStandardEvents)
                            {
                                ICommonEvent? first = null;
                                ICommonEvent? last = null;
                                var computeDiscretizedEvents = k.ComputeDiscretizedEvents(
                                    discretizingInterval,
                                    discretizingAccuracy);
                                foreach (ICommonEvent discretizedEvent in computeDiscretizedEvents)
                                {
                                    first ??= discretizedEvent;
                                    last = discretizedEvent;
                                    var valueTuple = (discretizedEvent.StartTime, discretizedEvent.EndTime);
                                    discretizingTargetStandardEvents.Add(valueTuple, discretizedEvent);
                                }

                                //if (first != null) boundedDiscretizingTargetStandardEvents.Add(first);
                                //if (last != null) boundedDiscretizingTargetStandardEvents.Add(last);
                            }

                            var list = new List<ICommonEvent>();
                            foreach (var relative in discretizedRelatives.ToList())
                            {
                                var key = relative.Key;
                                var relativeEvent = relative.Value;
                                if (discretizingTargetStandardEvents.TryGetValue(key, out var completelyCoincidentEvent))
                                {
                                    // exact coincident
                                    var eventType = completelyCoincidentEvent.EventType;
                                    var newStart =
                                        eventType.ComputeRelative(completelyCoincidentEvent.Start, relativeEvent.Start, discretizingAccuracy);
                                    var newEnd =
                                        eventType.ComputeRelative(completelyCoincidentEvent.End, relativeEvent.End, discretizingAccuracy);
                                    completelyCoincidentEvent.Start = newStart;
                                    completelyCoincidentEvent.End = newEnd;
                                    discretizingTargetStandardEvents.Remove(key);
                                    list.Add(completelyCoincidentEvent);
                                }
                                else
                                {
                                    var bounded =/*boundedDiscretizingTargetStandardEvents*/discretizingTargetStandardEvents.Values.FirstOrDefault(k =>
                                        k.StartTime <= relative.Key.EndTime && k.EndTime >= relative.Key.StartTime);
                                    if (bounded == null)
                                    {
                                        // nothing
                                        var lastValue = SpriteExtensions.ComputeFrame(allThisTypeStandardEvents,
                                            relative.Value.EventType, relative.Key.StartTime, discretizingAccuracy);
                                        var commonEvent = CommonEvent.Create(targetStdType, EasingType.Linear,
                                            relative.Key.StartTime, relative.Key.EndTime,
                                            @event.EventType.ComputeRelative(lastValue, relative.Value.Start, discretizingAccuracy),
                                            @event.EventType.ComputeRelative(lastValue, relative.Value.End, discretizingAccuracy));
                                        list.Add(commonEvent);
                                    }
                                    else
                                    {
                                        if (relative.Key.StartTime <= bounded.EndTime && relative.Key.EndTime > bounded.EndTime)
                                        {
                                            // kvp: relative; bounded: absolute
                                            // absolute: !____|-?- (bounded)
                                            // relative:    |___!  (kvp) (0~?~100)
                                            //           0  1 2 3
                                            var absoluteFrame1 = SpriteExtensions.ComputeFrame(allThisTypeStandardEvents,
                                                targetStdType, relative.Key.StartTime, discretizingAccuracy);
                                            var computedFrame1 = @event.EventType.ComputeRelative(absoluteFrame1, relative.Value.Start, discretizingAccuracy);
                                            if (!relative.Key.StartTime.Equals(bounded.StartTime))
                                            {
                                                var newEvent0_1 = CommonEvent.Create(targetStdType, EasingType.Linear,
                                                    bounded.StartTime, relative.Key.StartTime,
                                                    bounded.Start.ToArray(), computedFrame1);
                                                AddMiniUnitEvent(newEvent0_1, list);
                                                discretizingTargetStandardEvents.Remove((bounded.StartTime, relative.Key.StartTime));
                                            }

                                            double[] computedFrame2;
                                            var relativeFrame2 = @event.ComputeFrame(bounded.EndTime, discretizingAccuracy); //get
                                            computedFrame2 = @event.EventType.ComputeRelative(bounded.End, relativeFrame2, discretizingAccuracy);
                                            if (!relative.Key.StartTime.Equals(bounded.EndTime))
                                            {
                                                var newEvent1_2 = CommonEvent.Create(targetStdType, EasingType.Linear,
                                                    relative.Key.StartTime, bounded.EndTime,
                                                    computedFrame1, computedFrame2);
                                                AddMiniUnitEvent(newEvent1_2, list);
                                                discretizingTargetStandardEvents.Remove((relative.Key.StartTime, bounded.EndTime));
                                            }

                                            var absoluteFrame3 = SpriteExtensions.ComputeFrame(allThisTypeStandardEvents,
                                                targetStdType, relative.Key.EndTime, discretizingAccuracy);
                                            var computedFrame3 = @event.EventType.ComputeRelative(absoluteFrame3, relative.Value.End, discretizingAccuracy);
                                            if (!relative.Key.EndTime.Equals(bounded.EndTime))
                                            {
                                                var newEvent2_3 = CommonEvent.Create(targetStdType, EasingType.Linear,
                                                    bounded.EndTime, relative.Key.EndTime,
                                                    computedFrame2, computedFrame3);
                                                AddMiniUnitEvent(newEvent2_3, list);
                                                discretizingTargetStandardEvents.Remove((bounded.EndTime, relative.Key.EndTime));
                                            }

                                        }
                                        else if (relative.Key.EndTime >= bounded.StartTime && relative.Key.StartTime < bounded.StartTime)
                                        {
                                            // kvp: relative; bounded: absolute
                                            // absolute: ?-|____! (bounded)
                                            // relative: !___|    (kvp) (0~?~100)
                                            //           0 1 2  3
                                            var absoluteFrame0 = SpriteExtensions.ComputeFrame(allThisTypeStandardEvents,
                                                targetStdType, relative.Key.StartTime, discretizingAccuracy);
                                            var computedFrame0 = @event.EventType.ComputeRelative(absoluteFrame0, relative.Value.Start, discretizingAccuracy);
                                            var relativeFrame1 = @event.ComputeFrame(bounded.StartTime, discretizingAccuracy);
                                            var computedFrame1 = @event.EventType.ComputeRelative(bounded.Start, relativeFrame1, discretizingAccuracy);

                                            if (!relative.Key.StartTime.Equals(bounded.StartTime))
                                            {
                                                var newEvent0_1 = CommonEvent.Create(targetStdType, EasingType.Linear,
                                                    relative.Key.StartTime, bounded.StartTime,
                                                    computedFrame0, computedFrame1);
                                                AddMiniUnitEvent(newEvent0_1, list);
                                                discretizingTargetStandardEvents.Remove((relative.Key.StartTime, bounded.StartTime));
                                            }

                                            double[] computedFrame2;
                                            var absoluteFrame2 = SpriteExtensions.ComputeFrame(allThisTypeStandardEvents,
                                                targetStdType, relative.Key.EndTime, discretizingAccuracy);
                                            computedFrame2 = @event.EventType.ComputeRelative(absoluteFrame2, relative.Value.End, discretizingAccuracy);
                                            if (!relative.Key.EndTime.Equals(bounded.StartTime))
                                            {
                                                var newEvent1_2 = CommonEvent.Create(targetStdType, EasingType.Linear,
                                                 bounded.StartTime, relative.Key.EndTime,
                                                 computedFrame1, computedFrame2);
                                                AddMiniUnitEvent(newEvent1_2, list);
                                                discretizingTargetStandardEvents.Remove((bounded.StartTime, relative.Key.EndTime));
                                            }

                                            //if (!grouping
                                            //    .Where(k => k != @event)
                                            //    .Any(k => k.StartTime <= bounded.EndTime && k.EndTime >= bounded.StartTime))
                                            if (!relative.Key.EndTime.Equals(bounded.EndTime) &&
                                                //!grouping
                                                //    .Where(k => k != @event)
                                                //    .Any(k => k.StartTime <= bounded.EndTime &&
                                                //              k.EndTime >= bounded.StartTime) && 
                                                !discretizedRelatives
                                                    .Where(k => k.Value != relative.Value)
                                                    .Any(k => k.Key.StartTime <= bounded.EndTime &&
                                                              k.Key.EndTime >= bounded.StartTime))
                                            {
                                                var computedFrame3 =
                                                    @event.EventType.ComputeRelative(bounded.End, relative.Value.End, discretizingAccuracy);
                                                var newEvent2_3 = CommonEvent.Create(targetStdType, EasingType.Linear,
                                                    relative.Key.EndTime, bounded.EndTime,
                                                    computedFrame2, computedFrame3);
                                                AddMiniUnitEvent(newEvent2_3, list);
                                                discretizingTargetStandardEvents.Remove((relative.Key.EndTime, bounded.EndTime));
                                            }
                                        }
                                        else if (relative.Key.StartTime >= bounded.StartTime && relative.Key.EndTime <= bounded.EndTime)
                                        {
                                            // kvp: relative; bounded: absolute
                                            // absolute: !______!  (bounded)
                                            // relative: |_|__|_|  (kvp) (0~?~100)
                                            //           0 1  2 3
                                            var computedFrame0 = @event.EventType.ComputeRelative(bounded.Start, relative.Value.Start, discretizingAccuracy);

                                            var absoluteFrame1 = SpriteExtensions.ComputeFrame(allThisTypeStandardEvents,
                                                targetStdType, relative.Key.StartTime, discretizingAccuracy);
                                            var computedFrame1 = @event.EventType.ComputeRelative(absoluteFrame1, relative.Value.Start, discretizingAccuracy);

                                            if (!relative.Key.StartTime.Equals(bounded.StartTime))
                                            {
                                                var newEvent0_1 = CommonEvent.Create(targetStdType, EasingType.Linear,
                                                    bounded.StartTime, relative.Key.StartTime,
                                                    computedFrame0, computedFrame1);
                                                AddMiniUnitEvent(newEvent0_1, list);
                                                discretizingTargetStandardEvents.Remove((bounded.StartTime, relative.Key.StartTime));
                                            }

                                            var absoluteFrame2 = SpriteExtensions.ComputeFrame(allThisTypeStandardEvents,
                                                targetStdType, relative.Key.EndTime, discretizingAccuracy);
                                            var computedFrame2 = @event.EventType.ComputeRelative(absoluteFrame2, relative.Value.End, discretizingAccuracy);

                                            var newEvent1_2 = CommonEvent.Create(targetStdType, EasingType.Linear,
                                                relative.Key.StartTime, relative.Key.EndTime,
                                                computedFrame1, computedFrame2);
                                            AddMiniUnitEvent(newEvent1_2, list);
                                            discretizingTargetStandardEvents.Remove((relative.Key.StartTime, relative.Key.EndTime));

                                            if (!relative.Key.EndTime.Equals(bounded.EndTime) &&
                                                //!grouping
                                                //    .Where(k => k != @event)
                                                //    .Any(k => k.StartTime <= bounded.EndTime &&
                                                //              k.EndTime >= bounded.StartTime) &&
                                                !discretizedRelatives
                                                    .Where(k => k.Value != relative.Value)
                                                    .Any(k => k.Key.StartTime <= bounded.EndTime &&
                                                              k.Key.EndTime >= bounded.StartTime))
                                            {
                                                var computedFrame3 =
                                                    @event.EventType.ComputeRelative(bounded.End, relative.Value.End,
                                                        3);
                                                var newEvent2_3 = CommonEvent.Create(targetStdType, EasingType.Linear,
                                                    relative.Key.EndTime, bounded.EndTime,
                                                    computedFrame2, computedFrame3);
                                                AddMiniUnitEvent(newEvent2_3, list);
                                                discretizingTargetStandardEvents.Remove((relative.Key.EndTime, bounded.EndTime));
                                            }

                                        }
                                        else
                                        {
                                            throw new Exception("no way!");
                                        }

                                        //Console.WriteLine(bounded.StartTime + "," + bounded.EndTime + "<->" + kvp.Key);
                                        discretizingTargetStandardEvents.Remove((bounded.StartTime, bounded.EndTime));
                                        //boundedDiscretizingTargetStandardEvents.Remove(bounded);
                                    }
                                }

                                discretizedRelatives.Remove(key);
                            }

                            foreach (var commonEvent in list)
                            {
                                sprite.Events.Add(commonEvent);
                            }

                            foreach (var value in discretizingTargetStandardEvents.Values)
                            {
                                if (value.StartTime >= endTime)
                                {
                                    for (int i = 0; i < value.EventType.Size; i++)
                                    {
                                        value.Start[i] += @event.End[i]; // offset
                                        value.End[i] += @event.End[i];
                                    }
                                }

                                sprite.Events.Add(value);
                            }

                            sprite.Events.Remove(@event);
                            list.Clear();
                            foreach (var targetStandardEvent in targetStandardEvents)
                            {
                                sprite.Events.Remove(targetStandardEvent);
                            }

                            var nextEvents = allThisTypeStandardEvents
                                .Where(k => k.StartTime >= endTime)
                                .ToList();
                            foreach (var commonEvent in nextEvents)
                            {
                                for (int i = 0; i < commonEvent.EventType.Size; i++)
                                {
                                    commonEvent.Start[i] += @event.End[i]; // offset
                                    commonEvent.End[i] += @event.End[i];
                                }
                            }
                        }
                    }
                    finally
                    {
                        sprite.WriteScriptAsync(Console.Out).Wait();
                    }
                }
            }
        }

        private static void AddEventDirectly(IEventHost sprite, ICommonEvent newEvent,
            int discretizingInterval,
            int? discretizingAccuracy)
        {
            if (newEvent.Easing.TryGetEasingType() == null)
            {
                var de = newEvent.ComputeDiscretizedEvents(false,
                    discretizingInterval,
                    discretizingAccuracy);
                foreach (var commonEvent in de)
                {
                    sprite.Events.Add(commonEvent);
                }
            }
            else
            {
                sprite.Events.Add(newEvent);
            }
        }

        private static void AddMiniUnitEvent(ICommonEvent e, ICollection<ICommonEvent> list)
        {
            if (e.IsStatic && e.StartTime.Equals(e.EndTime))
            {

            }

            else if (e.IsStatic && e.StartTime.Equals(e.EndTime) &&
                 (list.Any(k => k.StartTime.Equals(e.StartTime)
                                && e.Start.SequenceEqual(k.Start)) ||
                  list.Any(k => k.EndTime.Equals(e.StartTime)
                                && e.Start.SequenceEqual(k.End))))

            {
            }
            else if (list.Any(k => k.StartTime.Equals(e.StartTime)
                                   && k.EndTime.Equals(e.EndTime)))
            {

            }
            else
            {
                list.Add(e);
            }
        }
    }
}