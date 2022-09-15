using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Coosu.Shared;
using Coosu.Shared.Mathematics;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensions.Computing;

public static class SpriteExtensions
{
    public static TimeRange ComputeInvisibleRange(this Sprite sprite, out HashSet<BasicEvent> keyEvents)
    {
        var obsoleteList = new TimeRange();
        keyEvents = new HashSet<BasicEvent>();
        var possibleList = sprite.Events
            .Where(k => k.EventType == EventTypes.Fade ||
                        k.EventType == EventTypes.Scale ||
                        k.EventType == EventTypes.Vector)
            .Cast<BasicEvent>();

        if (!possibleList.Any())
            return obsoleteList;

        var dic = new Dictionary<string, EventContext>
        {
            [EventTypes.Fade.Flag] = new(),
            [EventTypes.Scale.Flag] = new(),
            [EventTypes.Vector.Flag] = new()
        };

        var ineffectiveDictionary = EventExtensions.IneffectiveDictionary;
        var triggers = sprite.TriggerList.Count == 0
            ? EmptyArray<Trigger>.Value
            : sprite.TriggerList
                .Where(k =>
                    k.Events.Any(o => ineffectiveDictionary.ContainsKey(o.EventType.Flag))
                )
                .ToArray();
        var loops = sprite.LoopList.Count == 0
            ? EmptyArray<Loop>.Value
            : sprite.LoopList
                .Where(k =>
                    k.Events.Any(o => ineffectiveDictionary.ContainsKey(o.EventType.Flag))
                )
                .ToArray();

        foreach (var e in possibleList)
        {
            var flag = e.EventType.Flag;
#if DEBUG
            if (flag == "F")
            {
            }
#endif

            dic[flag].Count++;
            // 最早的event晚于最小开始时间，默认加这一段
            var starts = e.GetStarts();
            var ends = e.GetEnds();
            var ineffectiveValue = e.GetIneffectiveValue();
            var isStartsIneffective = starts.SequenceEqual(ineffectiveValue);
            var isEndsIneffective = ends.SequenceEqual(ineffectiveValue);

            if (dic[flag].Count == 1 &&
                isStartsIneffective &&
                e.StartTime > sprite.MinTime())
            {
                if (e.IsStartsEqualsEnds())
                    e.AdjustTiming(sprite.MinTime() - e.StartTime); // todo: stop changing here

                dic[flag].StartTime = sprite.MinTime();
                dic[flag].IsFadingOut = true;
                keyEvents.Add(e);
            }

            // event.Start和End都为无用值时，开始计时
            if (isStartsIneffective &&
                isEndsIneffective &&
                dic[flag].IsFadingOut == false)
            {
                dic[flag].StartTime = e.StartTime;
                dic[flag].IsFadingOut = true;
                keyEvents.Add(e);
            }
            // event.End为无用值时，开始计时
            else if (isEndsIneffective &&
                     dic[flag].IsFadingOut == false)
            {
                dic[flag].StartTime = e.EndTime;
                dic[flag].IsFadingOut = true;
                keyEvents.Add(e);
            }
            else if (dic[flag].IsFadingOut)
            {
                if (isStartsIneffective &&
                    isEndsIneffective)
                    continue;
                AddTimeRage(dic[flag].StartTime, e.StartTime);
                dic[flag].IsFadingOut = false;
                dic[flag].StartTime = double.MinValue;
                keyEvents.Add(e);
            }
        }

        // 可能存在遍历完后所有event后，仍存在某一项>0（后面还有别的event，算无用）
        foreach (var pair in dic
                     .Where(k => k.Value.IsFadingOut)
                     .OrderBy(k => k.Value.StartTime))
        {
            if (Precision.AlmostEquals(pair.Value.StartTime, sprite.MaxTime()))
                break;
            AddTimeRage(pair.Value.StartTime, sprite.MaxTime());
            break;
        }

        void AddTimeRage(double startTime, double endTime)
        {
            if (triggers.Length > 0 &&
                triggers.Any(k => endTime >= k.StartTime && startTime <= k.EndTime) ||
                loops.Length > 0 &&
                loops.Any(k => endTime >= k.StartTime && startTime <= k.EndTime))
            {
                return;
            }

            obsoleteList.Add(startTime, endTime);
        }

        return obsoleteList;
    }

    private class EventContext
    {
        public int Count { get; set; } = 0;
        public bool IsFadingOut { get; set; } = false;
        public double StartTime { get; set; } = double.MinValue;
    }
}