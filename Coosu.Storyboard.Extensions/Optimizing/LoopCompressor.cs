using System;
using System.Collections.Generic;
using System.Linq;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensions.Optimizing;

/// <summary>
/// Finds time-shifted command periods and commits only the shortest serialized loop candidate.
/// </summary>
internal static class LoopCompressor
{
    public static void Reduce(Sprite sprite, Func<Sprite, int> measureSerializedLength)
    {
        if (sprite == null) throw new ArgumentNullException(nameof(sprite));
        if (measureSerializedLength == null) throw new ArgumentNullException(nameof(measureSerializedLength));

        // Commands already nested by the author stay untouched. In particular, moving the same
        // command family around a trigger can change osu!'s command evaluation order.
        var blockedTypes = new HashSet<EventType>(
            sprite.LoopList.SelectMany(k => k.Events)
                .Concat(sprite.TriggerList.SelectMany(k => k.Events))
                .Select(k => k.EventType));

        ReduceRepeatedPeriods(sprite, blockedTypes, measureSerializedLength);
        RebaseLargeAbsoluteTimes(sprite, measureSerializedLength);
    }

    private static void ReduceRepeatedPeriods(Sprite sprite, HashSet<EventType> blockedTypes,
        Func<Sprite, int> measureSerializedLength)
    {
        while (true)
        {
            var sourceLength = measureSerializedLength(sprite);
            LoopCandidate? bestCandidate = null;
            var bestLength = sourceLength;

            foreach (var plan in FindRepeatPlans(sprite, blockedTypes))
            {
                foreach (var candidate in SelectTypeCandidates(sprite, plan, measureSerializedLength))
                {
                    var candidateSprite = (Sprite)sprite.Clone();
                    Apply(candidateSprite, candidate);
                    var candidateLength = measureSerializedLength(candidateSprite);
                    if (candidateLength >= bestLength) continue;

                    bestLength = candidateLength;
                    bestCandidate = candidate;
                }
            }

            if (bestCandidate == null || bestLength >= sourceLength) return;
            Apply(sprite, bestCandidate);
        }
    }

    private static IEnumerable<RepeatPlan> FindRepeatPlans(Sprite sprite, HashSet<EventType> blockedTypes)
    {
        var events = sprite.Events
            .OfType<BasicEvent>()
            .Where(k => !blockedTypes.Contains(k.EventType))
            .OrderBy(k => k, EventSequenceComparer.Instance)
            .ToArray();
        if (events.Length < 2) yield break;

        var geometries = new HashSet<PeriodGeometry>();
        for (var i = 0; i < events.Length - 1; i++)
        {
            for (var j = i + 1; j < events.Length; j++)
            {
                var start = SerializedTime(events[i].StartTime);
                var period = SerializedTime(events[j].StartTime) - start;
                if (period <= 0 || !AreShiftedCopies(events[i], events[j], period)) continue;

                geometries.Add(new PeriodGeometry(start, period));
            }
        }

        var groups = events
            .GroupBy(k => k.EventType)
            .ToDictionary(k => k.Key, k => k.OrderBy(o => o, EventSequenceComparer.Instance).ToArray());

        foreach (var geometry in geometries.OrderBy(k => k.Start).ThenBy(k => k.Period))
        {
            var repeatTypes = groups
                .Select(k => AnalyzeType(k.Key, k.Value, geometry))
                .Where(k => k != null)
                .Cast<RepeatType>()
                .ToArray();
            if (repeatTypes.Length == 0) continue;

            var maxCount = repeatTypes.Max(k => k.RepeatCount);
            for (var count = 2; count <= maxCount; count++)
            {
                var capableTypes = repeatTypes.Where(k => k.RepeatCount >= count).ToArray();
                if (capableTypes.Any(k => k.ReachesPeriodEnd))
                    yield return new RepeatPlan(geometry.Start, geometry.Period, count, capableTypes);
            }
        }
    }

    private static RepeatType? AnalyzeType(EventType eventType, BasicEvent[] events, PeriodGeometry geometry)
    {
        var start = geometry.Start;
        var period = geometry.Period;
        var firstEnd = start + period;
        var template = EventsInWindow(events, start, firstEnd);
        if (template.Length == 0 || template.Any(k => SerializedTime(k.EndTime) > firstEnd)) return null;

        // A command beginning before the loop but ending inside it would overlap the nested copy.
        if (events.Any(k => SerializedTime(k.StartTime) < start && SerializedTime(k.EndTime) > start))
            return null;

        var repeatCount = 1;
        while (true)
        {
            var windowStart = start + repeatCount * period;
            var windowEnd = windowStart + period;
            var copy = EventsInWindow(events, windowStart, windowEnd);
            if (!AreShiftedBlocks(template, copy, repeatCount * period) ||
                copy.Any(k => SerializedTime(k.EndTime) > windowEnd))
                break;

            repeatCount++;
        }

        if (repeatCount < 2) return null;
        var reachesPeriodEnd = template.Max(k => SerializedTime(k.EndTime)) == firstEnd;
        return new RepeatType(eventType, repeatCount, reachesPeriodEnd);
    }

    private static IEnumerable<LoopCandidate> SelectTypeCandidates(Sprite sprite, RepeatPlan plan,
        Func<Sprite, int> measureSerializedLength)
    {
        var loopEnd = plan.Start + plan.Period * plan.Count;
        var availableTypes = plan.Types.Where(type =>
            sprite.LoopList.All(loop => !loop.Events.Any(k => k.EventType == type.EventType) ||
                                        !RangesOverlap(plan.Start, loopEnd, SerializedTime(loop.StartTime),
                                            SerializedTime(loop.OuterMaxTime())))).ToArray();

        // A loop's duration is its inner maximum end time. Seed each candidate with one type that
        // reaches the period boundary, then add every other type only when its exact text delta wins.
        foreach (var boundaryType in availableTypes.Where(k => k.ReachesPeriodEnd))
        {
            var selectedTypes = new HashSet<EventType> { boundaryType.EventType };
            var current = CreateCandidate(plan, selectedTypes);
            var currentLength = MeasureCandidate(sprite, current, measureSerializedLength);

            foreach (var type in availableTypes.Where(k => k.EventType != boundaryType.EventType))
            {
                var extendedTypes = new HashSet<EventType>(selectedTypes) { type.EventType };
                var extended = CreateCandidate(plan, extendedTypes);
                var extendedLength = MeasureCandidate(sprite, extended, measureSerializedLength);
                if (extendedLength >= currentLength) continue;

                selectedTypes = extendedTypes;
                current = extended;
                currentLength = extendedLength;
            }

            yield return current;
        }
    }

    private static int MeasureCandidate(Sprite sprite, LoopCandidate candidate,
        Func<Sprite, int> measureSerializedLength)
    {
        var clone = (Sprite)sprite.Clone();
        Apply(clone, candidate);
        return measureSerializedLength(clone);
    }

    private static LoopCandidate CreateCandidate(RepeatPlan plan, IEnumerable<EventType> types) =>
        new(plan.Start, plan.Period, plan.Count, types.OrderBy(k => k.Index).ToArray());

    private static BasicEvent[] EventsInWindow(IEnumerable<BasicEvent> events, double start, double end) =>
        events.Where(k => SerializedTime(k.StartTime) >= start && SerializedTime(k.StartTime) < end)
            .OrderBy(k => k, EventSequenceComparer.Instance)
            .ToArray();

    private static bool AreShiftedBlocks(BasicEvent[] template, BasicEvent[] copy, double shift)
    {
        if (template.Length != copy.Length) return false;
        for (var i = 0; i < template.Length; i++)
        {
            if (!AreShiftedCopies(template[i], copy[i], shift)) return false;
        }

        return true;
    }

    private static bool AreShiftedCopies(BasicEvent first, BasicEvent second, double shift)
    {
        if (first.EventType != second.EventType ||
            first.Easing.TryGetEasingType() != second.Easing.TryGetEasingType() ||
            SerializedTime(second.StartTime) - SerializedTime(first.StartTime) != shift ||
            SerializedTime(second.EndTime) - SerializedTime(first.EndTime) != shift)
            return false;

        return HaveEquivalentValues(first, second);
    }

    private static bool HaveEquivalentValues(BasicEvent first, BasicEvent second)
    {
        if (first is Parameter || second is Parameter)
            return first.Values.SequenceEqual(second.Values);

        var size = first.EventType.Size;
        for (var i = 0; i < size; i++)
        {
            var firstStart = ValueAt(first, i, first.DefaultValue);
            var secondStart = ValueAt(second, i, second.DefaultValue);
            if (!firstStart.Equals(secondStart)) return false;

            var firstEnd = ValueAt(first, i + size, firstStart);
            var secondEnd = ValueAt(second, i + size, secondStart);
            if (!firstEnd.Equals(secondEnd)) return false;
        }

        return true;
    }

    private static double ValueAt(BasicEvent keyEvent, int index, double fallback) =>
        index < keyEvent.Values.Count ? keyEvent.Values[index] : fallback;

    private static bool RangesOverlap(double firstStart, double firstEnd, double secondStart, double secondEnd) =>
        firstStart < secondEnd && secondStart < firstEnd;

    private static void Apply(Sprite sprite, LoopCandidate candidate)
    {
        var end = candidate.Start + candidate.Period * candidate.Count;
        var selectedTypes = new HashSet<EventType>(candidate.Types);
        var sourceEvents = sprite.Events
            .OfType<BasicEvent>()
            .Where(k => selectedTypes.Contains(k.EventType) &&
                        SerializedTime(k.StartTime) >= candidate.Start &&
                        SerializedTime(k.StartTime) < end)
            .ToArray();
        var templateEvents = sourceEvents
            .Where(k => SerializedTime(k.StartTime) < candidate.Start + candidate.Period)
            .OrderBy(k => k, EventSequenceComparer.Instance)
            .ToArray();

        var loop = new Loop(candidate.Start, candidate.Count);
        foreach (var sourceEvent in templateEvents)
        {
            var innerEvent = (BasicEvent)sourceEvent.Clone();
            innerEvent.StartTime = SerializedTime(sourceEvent.StartTime) - candidate.Start;
            innerEvent.EndTime = SerializedTime(sourceEvent.EndTime) - candidate.Start;
            loop.AddEvent(innerEvent);
        }

        foreach (var sourceEvent in sourceEvents)
            sprite.RemoveEvent(sourceEvent);
        sprite.AddLoop(loop);
    }

    private static void RebaseLargeAbsoluteTimes(Sprite sprite, Func<Sprite, int> measureSerializedLength)
    {
        var blockedTypes = new HashSet<EventType>(
            sprite.LoopList.SelectMany(k => k.Events)
                .Concat(sprite.TriggerList.SelectMany(k => k.Events))
                .Select(k => k.EventType));
        var events = sprite.Events
            .OfType<BasicEvent>()
            .Where(k => SerializedTime(k.StartTime) > 1000 && !blockedTypes.Contains(k.EventType))
            .OrderBy(k => k, EventSequenceComparer.Instance)
            .ToArray();
        if (events.Length <= 1) return;

        var candidate = (Sprite)sprite.Clone();
        var candidateEvents = candidate.Events
            .OfType<BasicEvent>()
            .Where(k => SerializedTime(k.StartTime) > 1000 && !blockedTypes.Contains(k.EventType))
            .OrderBy(k => k, EventSequenceComparer.Instance)
            .ToArray();
        var start = SerializedTime(candidateEvents[0].StartTime);
        var loop = new Loop(start, 1);
        foreach (var candidateEvent in candidateEvents)
        {
            candidate.RemoveEvent(candidateEvent);
            candidateEvent.StartTime = SerializedTime(candidateEvent.StartTime) - start;
            candidateEvent.EndTime = SerializedTime(candidateEvent.EndTime) - start;
            loop.AddEvent(candidateEvent);
        }

        candidate.AddLoop(loop);
        if (measureSerializedLength(candidate) >= measureSerializedLength(sprite)) return;

        var sourceStart = SerializedTime(events[0].StartTime);
        var sourceLoop = new Loop(sourceStart, 1);
        foreach (var sourceEvent in events)
        {
            sprite.RemoveEvent(sourceEvent);
            sourceEvent.StartTime = SerializedTime(sourceEvent.StartTime) - sourceStart;
            sourceEvent.EndTime = SerializedTime(sourceEvent.EndTime) - sourceStart;
            sourceLoop.AddEvent(sourceEvent);
        }

        sprite.AddLoop(sourceLoop);
    }

    private static double SerializedTime(double value) => Math.Round(value);

    private sealed class RepeatType
    {
        public EventType EventType { get; }
        public int RepeatCount { get; }
        public bool ReachesPeriodEnd { get; }

        public RepeatType(EventType eventType, int repeatCount, bool reachesPeriodEnd)
        {
            EventType = eventType;
            RepeatCount = repeatCount;
            ReachesPeriodEnd = reachesPeriodEnd;
        }
    }

    private sealed class RepeatPlan
    {
        public double Start { get; }
        public double Period { get; }
        public int Count { get; }
        public IReadOnlyList<RepeatType> Types { get; }

        public RepeatPlan(double start, double period, int count, IReadOnlyList<RepeatType> types)
        {
            Start = start;
            Period = period;
            Count = count;
            Types = types;
        }
    }

    private sealed class LoopCandidate
    {
        public double Start { get; }
        public double Period { get; }
        public int Count { get; }
        public IReadOnlyList<EventType> Types { get; }

        public LoopCandidate(double start, double period, int count, IReadOnlyList<EventType> types)
        {
            Start = start;
            Period = period;
            Count = count;
            Types = types;
        }
    }

    private readonly struct PeriodGeometry : IEquatable<PeriodGeometry>
    {
        public double Start { get; }
        public double Period { get; }

        public PeriodGeometry(double start, double period)
        {
            Start = start;
            Period = period;
        }

        public bool Equals(PeriodGeometry other) => Start.Equals(other.Start) && Period.Equals(other.Period);
        public override bool Equals(object? obj) => obj is PeriodGeometry other && Equals(other);
        public override int GetHashCode()
        {
            unchecked
            {
                return (Start.GetHashCode() * 397) ^ Period.GetHashCode();
            }
        }
    }
}
