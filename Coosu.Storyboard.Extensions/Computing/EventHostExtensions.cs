using System;
using System.Collections.Generic;
using System.Linq;
using Coosu.Shared;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Extensions.Optimizing;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Extensions.Computing
{
    public static class EventHostExtensions
    {
        public static void Expand(this IDetailedEventHost host)
        {
            if (host is Sprite sprite)
            {
                if (sprite.TriggerList.Count > 0)
                {
                    foreach (var t in sprite.TriggerList)
                        t.Expand();
                }

                if (sprite.LoopList.Count > 0)
                {
                    foreach (var loop in sprite.LoopList)
                    {
                        loop.Expand();
                        var loopCount = loop.LoopCount;
                        var startTime = loop.StartTime;
                        for (int count = 0; count < loopCount; count++)
                        {
                            var fixedStartTime = startTime + (count * loop.MaxTime);
                            foreach (var e in loop.Events)
                            {
                                sprite.AddEvent(
                                    BasicEvent.Create(e.EventType,
                                        e.Easing,
                                        fixedStartTime + e.StartTime, fixedStartTime + e.EndTime,
                                        e.Values.CloneAsList())
                                );
                            }
                        }
                    }

                    sprite.LoopList.Clear();
                }
            }

            var events = host.Events
                //.Where(k => k is CommonEvent)
                .Cast<BasicEvent>()?.GroupBy(k => k.EventType);
            if (events == null) return;
            foreach (var kv in events)
            {
                List<BasicEvent> list = kv.ToList();
                for (var i = 0; i < list.Count - 1; i++)
                {
                    if (list[i].IsStartsEqualsEnds()) // case 1
                    {
                        list[i].EndTime = list[i + 1].StartTime;
                    }

                    if (!list[i].EndTime.Equals(list[i + 1].StartTime)) // case 2
                    {
                        host.AddEvent(
                            BasicEvent.Create(list[i].EventType,
                                EasingType.Linear,
                                list[i].EndTime, list[i + 1].StartTime,
#if NET5_0_OR_GREATER
                                list[i].GetEndsSpan(), list[i].GetEndsSpan()
#else
                                list[i].GetEnds(), list[i].GetEnds()
#endif
                                )
                        );
                    }
                }
            }
        }

        /// <summary>
        /// 检查timing是否合法.
        /// </summary>
        public static void Examine(this IDetailedEventHost host, EventHandler<ProcessErrorEventArgs>? onError)
        {
            var events = host.Events.Where(k => k is not RelativeEvent).GroupBy(k => k.EventType);
            foreach (var kv in events)
            {
                var list = kv.ToArray();
                for (var i = 0; i < list.Length - 1; i++)
                {
                    IKeyEvent objNext = list[i + 1];
                    IKeyEvent objNow = list[i];
                    if (objNow.StartTime > objNow.EndTime)
                    {
                        var info = $"{{{objNow.GetHeaderString()}}}:\r\n" +
                                   $"Start time should not be larger than end time.";

                        var arg = new ProcessErrorEventArgs(host)
                        {
                            Message = info
                        };
                        onError?.Invoke(host, arg);
                        if (!arg.Continue)
                        {
                            return;
                        };
                    }
                    if (objNext.StartTime < objNow.EndTime)
                    {
                        var info = $"{{{objNow.GetHeaderString()}}} to {{{objNext.GetHeaderString()}}}:\r\n" +
                                   $"The previous object's end time should be larger than the next object's start time.";
                        var arg = new ProcessErrorEventArgs(host)
                        {
                            Message = info
                        };
                        onError?.Invoke(host, arg);
                        if (!arg.Continue)
                        {
                            return;
                        }
                    }
                }
            }

            if (host is not Sprite e)
                return;

            foreach (var item in e.LoopList)
            {
                Examine(item, onError);
            }

            foreach (var item in e.TriggerList)
            {
                Examine(item, onError);
            }
        }

        private const int AlgorithmSwitchThreshold = 30;
        public static int GetMaxTimeCount(this IDetailedEventHost eventHost)
        {
            var maxTime = eventHost.MaxTime;
            int sum = 0;
            if (eventHost is Sprite sprite)
            {
                if (sprite.LoopList.Count > 0)
                {
                    if (sprite.LoopList.Count < AlgorithmSwitchThreshold)
                    {
                        sum += sprite.LoopList.Count(k => k.OuterMaxTime.Equals(maxTime));
                    }
                    else
                    {
                        var orderBy2 = sprite.LoopList.OrderByDescending(k => k.OuterMaxTime);
                        foreach (var keyEvent in orderBy2)
                        {
                            if (keyEvent.OuterMaxTime >= maxTime) sum++;
                            else break;
                        }
                    }
                }

                if (sprite.TriggerList.Count > 0)
                {
                    if (sprite.TriggerList.Count < AlgorithmSwitchThreshold)
                    {
                        sum += sprite.TriggerList.Count(k => k.MaxTime.Equals(maxTime));
                    }
                    else
                    {
                        var orderBy3 = sprite.TriggerList.OrderByDescending(k => k.MaxTime);
                        foreach (var keyEvent in orderBy3)
                        {
                            if (keyEvent.MaxTime >= maxTime) sum++;
                            else break;
                        }
                    }
                }
            }

            if (eventHost.Events.Count < AlgorithmSwitchThreshold)
            {
                sum += eventHost.Events.Count(k => k.EndTime.Equals(maxTime));
            }
            else
            {
                var orderBy = eventHost.Events.OrderByDescending(k => k.EndTime);
                foreach (var keyEvent in orderBy)
                {
                    if (keyEvent.EndTime >= maxTime) sum++;
                    else break;
                }
            }

            return sum;
        }

        public static int GetMinTimeCount(this IDetailedEventHost eventHost)
        {
            var minTime = eventHost.MinTime;
            int sum = 0;
            if (eventHost is Sprite sprite)
            {
                if (sprite.LoopList.Count > 0)
                {
                    if (sprite.LoopList.Count < AlgorithmSwitchThreshold)
                    {
                        sum += sprite.LoopList.Count(k => k.OuterMinTime.Equals(minTime));
                    }
                    else
                    {
                        var orderBy2 = sprite.LoopList.OrderBy(k => k.OuterMinTime);
                        foreach (var keyEvent in orderBy2)
                        {
                            if (keyEvent.OuterMinTime <= minTime) sum++;
                            else break;
                        }
                    }
                }

                if (sprite.TriggerList.Count > 0)
                {
                    if (sprite.TriggerList.Count < AlgorithmSwitchThreshold)
                    {
                        sum += sprite.TriggerList.Count(k => k.MinTime.Equals(minTime));
                    }
                    else
                    {
                        var orderBy3 = sprite.TriggerList.OrderBy(k => k.MinTime);
                        foreach (var keyEvent in orderBy3)
                        {
                            if (keyEvent.MinTime <= minTime) sum++;
                            else break;
                        }
                    }
                }
            }

            if (eventHost.Events.Count < AlgorithmSwitchThreshold)
            {
                sum += eventHost.Events.Count(k => k.StartTime.Equals(minTime));
            }
            else
            {
                var orderBy = eventHost.Events.OrderBy(k => k.StartTime);
                foreach (var keyEvent in orderBy)
                {
                    if (keyEvent.StartTime <= minTime) sum++;
                    else break;
                }
            }

            return sum;
        }

        public static bool HasEffectiveTiming(this IDetailedEventHost eventHost)
        {
            if (eventHost.MaxTime < eventHost.MinTime)
                return false;
            if (eventHost.MaxTime.Equals(eventHost.MinTime))
                return false;
            return true;
        }

        public static List<float> ComputeFrame(this IEventHost eventHost, EventType eventType, float time, int? accuracy)
        {
            if (eventType.Size < 1) throw new ArgumentOutOfRangeException(nameof(eventType), eventType, "Only support sized event type.");
            var basicEvents = eventHost.Events
                .OrderBy(k => k.StartTime)
                .Where(k => k.EventType == eventType)
                .Cast<BasicEvent>()
                .ToList();
            if (basicEvents.Count == 0)
                return eventType.GetDefaultValue(eventHost as ICameraUsable)?.ToList() ??
                       throw new NotSupportedException(eventType.Flag + " doesn't have any default value.");

            if (time < basicEvents[0].StartTime)
                return basicEvents[0].GetStarts().ToList();

            var e = basicEvents.FirstOrDefault(k => k.StartTime <= time && k.EndTime > time);
            if (e != null) return BasicEventExtensions.ComputeFrame(e, time, accuracy);

            var lastE = basicEvents.Last(k => k.EndTime <= time);
            return lastE.GetEnds().ToList();
        }

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
            int discretizingInterval = 48,
            int? discretizingAccuracy = 3)
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
            var keyEvents = sprite.Events.ToList();
            foreach (var @event in keyEvents
                .Where(k => k is not RelativeEvent && k.Easing.TryGetEasingType() == null))
            {
                var eventList = @event.ComputeDiscretizedEvents(false,
                    discretizingInterval,
                    discretizingAccuracy);
                sprite.Events.Remove(@event);
                foreach (var keyEvent in eventList)
                {
                    sprite.Events.Add(keyEvent);
                }
            }
        }

        private static void ComputeRelativeEvents(IEventHost sprite,
            int discretizingInterval,
            int? discretizingAccuracy)
        {
            var keyEvents = sprite.Events.ToList();
            foreach (var grouping in keyEvents
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
                            .Where(k => k is BasicEvent &&
                                        k.Easing.TryGetEasingType() != null &&
                                        k.EventType == targetStdType)
                            .OrderBy(k => k.StartTime)
                            .Cast<BasicEvent>()
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
                            var newEvent = BasicEvent.Create(targetStdType, @event.Easing, startTime, endTime,
                                defaultValue,
                                @event.EventType.ComputeRelative(defaultValue, @event.Values));
                            AddEventDirectly(sprite, newEvent, discretizingInterval, discretizingAccuracy);
                            sprite.Events.Remove(@event);
                        }
                        else if (targetStandardEvents.Count == 0)
                        {
                            var lastValue = sprite.ComputeFrame(targetStdType, startTime, 0);
                            var newEvent = BasicEvent.Create(targetStdType, @event.Easing, startTime, endTime,
                                lastValue,
                                @event.EventType.ComputeRelative(lastValue, @event.Values));
                            AddEventDirectly(sprite, newEvent, discretizingInterval, discretizingAccuracy);
                            sprite.Events.Remove(@event);

                            var nextEvents = allThisTypeStandardEvents
                                .Where(k => k.StartTime >= endTime)
                                .ToList();
                            foreach (var basicEvent in nextEvents)
                            {
                                for (int i = 0; i < basicEvent.EventType.Size; i++)
                                {
                                    //var o = basicEvent.GetStartsValue(i);
                                    basicEvent.SetStartsValue(i, basicEvent.GetStartsValue(i) + @event.Values[i]); // offset
                                    basicEvent.SetEndsValue(i, basicEvent.GetEndsValue(i) + @event.Values[i]);
                                }
                            }
                        }
                        else
                        {
                            var discretizedRelatives = @event.ComputeDiscretizedEvents(false,
                                    discretizingInterval,
                                    null)
                                .Where(k => !k.IsStartsEqualsEnds())
                                .ToDictionary(k => (k.StartTime, k.EndTime), k => k);
                            endTime = (int)discretizedRelatives.Last().Key.EndTime;
                            var discretizingTargetStandardEvents =
                                new Dictionary<(double StartTime, double EndTime), IKeyEvent>();
                            //var boundedDiscretizingTargetStandardEvents = new HashSet<ICommonEvent>(); //todo: 算上因特殊情况被再度切割的？
                            foreach (BasicEvent k in targetStandardEvents)
                            {
                                IKeyEvent? first = null;
                                IKeyEvent? last = null;
                                var computeDiscretizedEvents = k.ComputeDiscretizedEvents(
                                    discretizingInterval,
                                    discretizingAccuracy);
                                foreach (IKeyEvent discretizedEvent in computeDiscretizedEvents)
                                {
                                    first ??= discretizedEvent;
                                    last = discretizedEvent;
                                    var valueTuple = (discretizedEvent.StartTime, discretizedEvent.EndTime);
                                    discretizingTargetStandardEvents.Add(valueTuple, discretizedEvent);
                                }

                                //if (first != null) boundedDiscretizingTargetStandardEvents.Add(first);
                                //if (last != null) boundedDiscretizingTargetStandardEvents.Add(last);
                            }

                            var list = new List<IKeyEvent>();
                            foreach (var relative in discretizedRelatives.ToList())
                            {
                                var key = relative.Key;
                                var relativeEvent = relative.Value;
                                if (discretizingTargetStandardEvents.TryGetValue(key, out var completelyCoincidentEvent))
                                {
                                    // exact coincident
                                    var eventType = completelyCoincidentEvent.EventType;
                                    var newStart =
                                        eventType.ComputeRelative(completelyCoincidentEvent.GetStarts().ToArray(), relativeEvent.GetStarts().ToArray(), discretizingAccuracy);
                                    var newEnd =
                                        eventType.ComputeRelative(completelyCoincidentEvent.GetEnds().ToArray(), relativeEvent.GetEnds().ToArray(), discretizingAccuracy);

                                    completelyCoincidentEvent.SetRawValues(newStart, newEnd);
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
                                        var lastValue = BasicEventExtensions.ComputeFrame(allThisTypeStandardEvents,
                                            relative.Value.EventType, relative.Key.StartTime, discretizingAccuracy);
                                        var basicEvent = BasicEvent.Create(targetStdType, EasingType.Linear,
                                            relative.Key.StartTime, relative.Key.EndTime,
                                            @event.EventType.ComputeRelative(lastValue, relative.Value.GetStarts().ToArray(), discretizingAccuracy),
                                            @event.EventType.ComputeRelative(lastValue, relative.Value.GetEnds().ToArray(), discretizingAccuracy));
                                        list.Add(basicEvent);
                                    }
                                    else
                                    {
                                        if (relative.Key.StartTime <= bounded.EndTime && relative.Key.EndTime > bounded.EndTime)
                                        {
                                            // kvp: relative; bounded: absolute
                                            // absolute: !____|-?- (bounded)
                                            // relative:    |___!  (kvp) (0~?~100)
                                            //           0  1 2 3
                                            var absoluteFrame1 = BasicEventExtensions.ComputeFrame(allThisTypeStandardEvents,
                                                targetStdType, relative.Key.StartTime, discretizingAccuracy);
                                            var computedFrame1 = @event.EventType.ComputeRelative(absoluteFrame1, relative.Value.GetStarts().ToArray(), discretizingAccuracy);
                                            if (!relative.Key.StartTime.Equals(bounded.StartTime))
                                            {
                                                var newEvent0_1 = BasicEvent.Create(targetStdType, EasingType.Linear,
                                                    bounded.StartTime, relative.Key.StartTime,
                                                    bounded.GetStarts().ToArray(), computedFrame1);
                                                AddMiniUnitEvent(newEvent0_1, list);
                                                discretizingTargetStandardEvents.Remove((bounded.StartTime, relative.Key.StartTime));
                                            }

                                            List<float> computedFrame2;
                                            var relativeFrame2 = @event.ComputeFrame(bounded.EndTime, discretizingAccuracy); //get
                                            computedFrame2 = @event.EventType.ComputeRelative(bounded.GetEnds().ToArray(), relativeFrame2, discretizingAccuracy);
                                            if (!relative.Key.StartTime.Equals(bounded.EndTime))
                                            {
                                                var newEvent1_2 = BasicEvent.Create(targetStdType, EasingType.Linear,
                                                    relative.Key.StartTime, bounded.EndTime,
                                                    computedFrame1, computedFrame2);
                                                AddMiniUnitEvent(newEvent1_2, list);
                                                discretizingTargetStandardEvents.Remove((relative.Key.StartTime, bounded.EndTime));
                                            }

                                            var absoluteFrame3 = BasicEventExtensions.ComputeFrame(allThisTypeStandardEvents,
                                                targetStdType, relative.Key.EndTime, discretizingAccuracy);
                                            var computedFrame3 = @event.EventType.ComputeRelative(absoluteFrame3, relative.Value.GetEnds().ToArray(), discretizingAccuracy);
                                            if (!relative.Key.EndTime.Equals(bounded.EndTime))
                                            {
                                                var newEvent2_3 = BasicEvent.Create(targetStdType, EasingType.Linear,
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
                                            var absoluteFrame0 = BasicEventExtensions.ComputeFrame(allThisTypeStandardEvents,
                                                targetStdType, relative.Key.StartTime, discretizingAccuracy);
                                            var computedFrame0 = @event.EventType.ComputeRelative(absoluteFrame0, relative.Value.GetStarts().ToArray(), discretizingAccuracy);
                                            var relativeFrame1 = @event.ComputeFrame(bounded.StartTime, discretizingAccuracy);
                                            var computedFrame1 = @event.EventType.ComputeRelative(bounded.GetStarts().ToArray(), relativeFrame1, discretizingAccuracy);

                                            if (!relative.Key.StartTime.Equals(bounded.StartTime))
                                            {
                                                var newEvent0_1 = BasicEvent.Create(targetStdType, EasingType.Linear,
                                                    relative.Key.StartTime, bounded.StartTime,
                                                    computedFrame0, computedFrame1);
                                                AddMiniUnitEvent(newEvent0_1, list);
                                                discretizingTargetStandardEvents.Remove((relative.Key.StartTime, bounded.StartTime));
                                            }

                                            List<float> computedFrame2;
                                            var absoluteFrame2 = BasicEventExtensions.ComputeFrame(allThisTypeStandardEvents,
                                                targetStdType, relative.Key.EndTime, discretizingAccuracy);
                                            computedFrame2 = @event.EventType.ComputeRelative(absoluteFrame2, relative.Value.GetEnds().ToArray(), discretizingAccuracy);
                                            if (!relative.Key.EndTime.Equals(bounded.StartTime))
                                            {
                                                var newEvent1_2 = BasicEvent.Create(targetStdType, EasingType.Linear,
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
                                                    @event.EventType.ComputeRelative(bounded.GetEnds().ToArray(), relative.Value.GetEnds().ToArray(), discretizingAccuracy);
                                                var newEvent2_3 = BasicEvent.Create(targetStdType, EasingType.Linear,
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
                                            var computedFrame0 = @event.EventType.ComputeRelative(bounded.GetStarts().ToArray(), relative.Value.GetStarts().ToArray(), discretizingAccuracy);

                                            var absoluteFrame1 = BasicEventExtensions.ComputeFrame(allThisTypeStandardEvents,
                                                targetStdType, relative.Key.StartTime, discretizingAccuracy);
                                            var computedFrame1 = @event.EventType.ComputeRelative(absoluteFrame1, relative.Value.GetStarts().ToArray(), discretizingAccuracy);

                                            if (!relative.Key.StartTime.Equals(bounded.StartTime))
                                            {
                                                var newEvent0_1 = BasicEvent.Create(targetStdType, EasingType.Linear,
                                                    bounded.StartTime, relative.Key.StartTime,
                                                    computedFrame0, computedFrame1);
                                                AddMiniUnitEvent(newEvent0_1, list);
                                                discretizingTargetStandardEvents.Remove((bounded.StartTime, relative.Key.StartTime));
                                            }

                                            var absoluteFrame2 = BasicEventExtensions.ComputeFrame(allThisTypeStandardEvents,
                                                targetStdType, relative.Key.EndTime, discretizingAccuracy);
                                            var computedFrame2 = @event.EventType.ComputeRelative(absoluteFrame2, relative.Value.GetEnds().ToArray(), discretizingAccuracy);

                                            var newEvent1_2 = BasicEvent.Create(targetStdType, EasingType.Linear,
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
                                                    @event.EventType.ComputeRelative(bounded.GetEnds().ToArray(), relative.Value.GetEnds().ToArray(),
                                                        3);
                                                var newEvent2_3 = BasicEvent.Create(targetStdType, EasingType.Linear,
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

                            foreach (var keyEvent in list)
                            {
                                sprite.Events.Add(keyEvent);
                            }

                            foreach (var value in discretizingTargetStandardEvents.Values)
                            {
                                if (value.StartTime >= endTime)
                                {
                                    for (int i = 0; i < value.EventType.Size; i++)
                                    {
                                        value.SetStartsValue(i, value.GetStartsValue(i) + @event.GetEndsValue(i)); // offset
                                        value.SetEndsValue(i, value.GetEndsValue(i) + @event.GetEndsValue(i));
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
                            foreach (var basicEvent in nextEvents)
                            {
                                for (int i = 0; i < basicEvent.EventType.Size; i++)
                                {
                                    basicEvent.SetStartsValue(i, basicEvent.GetStartsValue(i) + @event.GetEndsValue(i)); // offset
                                    basicEvent.SetEndsValue(i, basicEvent.GetEndsValue(i) + @event.GetEndsValue(i));
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

        private static void AddEventDirectly(IEventHost sprite, IKeyEvent newEvent,
            int discretizingInterval,
            int? discretizingAccuracy)
        {
            if (newEvent.Easing.TryGetEasingType() == null)
            {
                var de = newEvent.ComputeDiscretizedEvents(false,
                    discretizingInterval,
                    discretizingAccuracy);
                foreach (var keyEvent in de)
                {
                    sprite.Events.Add(keyEvent);
                }
            }
            else
            {
                sprite.Events.Add(newEvent);
            }
        }

        private static void AddMiniUnitEvent(IKeyEvent e, ICollection<IKeyEvent> list)
        {
            if (e.IsStartsEqualsEnds() && e.StartTime.Equals(e.EndTime))
            {

            }

            else if (e.IsStartsEqualsEnds() && e.StartTime.Equals(e.EndTime) &&
                 (list.Any(k => k.StartTime.Equals(e.StartTime)
                                && e.GetStarts().SequenceEqual(k.GetStarts())) ||
                  list.Any(k => k.EndTime.Equals(e.StartTime)
                                && e.GetStarts().SequenceEqual(k.GetEnds()))))

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