using System.Collections.Generic;
using System.Linq;
using Coosu.Storyboard.Events;

namespace Coosu.Storyboard.Extensions.Computing
{
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
                .Cast<BasicEvent>()
                .ToArray();

            if (possibleList.Length <= 0)
            {
                return (obsoleteList);
            }

            var dic = new Dictionary<EventType, EventContext>
            {
                [EventTypes.Fade] = new(),
                [EventTypes.Scale] = new(),
                [EventTypes.Vector] = new()
            };

            foreach (var e in possibleList)
            {
#if DEBUG
                if (e.EventType == EventTypes.Fade)
                {
                }
#endif

                dic[e.EventType].Count++;
                // 最早的event晚于最小开始时间，默认加这一段
                if (dic[e.EventType].Count == 1 &&
                    e.Start.SequenceEqual(e.GetIneffectiveValue()) &&
                    e.StartTime > sprite.MinTime)
                {
                    if (e.IsStartsEqualsEnds())
                        e.AdjustTiming(sprite.MinTime - e.StartTime); // todo: stop changing here
                    dic[e.EventType].StartTime = sprite.MinTime;
                    dic[e.EventType].IsFadingOut = true;
                    keyEvents.Add(e);
                }

                // event.Start和End都为无用值时，开始计时
                if (e.Start.SequenceEqual(e.GetIneffectiveValue()) &&
                    e.End.SequenceEqual(e.GetIneffectiveValue()) &&
                    dic[e.EventType].IsFadingOut == false)
                {
                    dic[e.EventType].StartTime = e.StartTime;
                    dic[e.EventType].IsFadingOut = true;
                    keyEvents.Add(e);
                }
                // event.End为无用值时，开始计时
                else if (e.End.SequenceEqual(e.GetIneffectiveValue()) &&
                         dic[e.EventType].IsFadingOut == false)
                {
                    dic[e.EventType].StartTime = e.EndTime;
                    dic[e.EventType].IsFadingOut = true;
                    keyEvents.Add(e);
                }
                else if (dic[e.EventType].IsFadingOut)
                {
                    if (e.Start.SequenceEqual(e.GetIneffectiveValue()) &&
                        e.End.SequenceEqual(e.GetIneffectiveValue()))
                        continue;
                    AddTimeRage(dic[e.EventType].StartTime, e.StartTime);
                    dic[e.EventType].IsFadingOut = false;
                    dic[e.EventType].StartTime = double.MinValue;
                    keyEvents.Add(e);
                }
            }

            // 可能存在遍历完后所有event后，仍存在某一项>0（后面还有别的event，算无用）
            foreach (var pair in dic
                .Where(k => k.Value.IsFadingOut && !k.Value.StartTime.Equals(sprite.MaxTime))
                .OrderBy(k => k.Value.StartTime))
            {
                AddTimeRage(pair.Value.StartTime, sprite.MaxTime);
                break;
            }

            void AddTimeRage(double startTime, double endTime)
            {
                if (sprite.TriggerList
                        .Where(k =>
                            k.Events
                                .Any(o => EventExtensions.IneffectiveDictionary.ContainsKey(o.EventType))
                        )
                        .Any(k => endTime >= k.StartTime && startTime <= k.EndTime)
                    ||
                    sprite.LoopList
                        .Where(k =>
                            k.Events
                                .Any(o => EventExtensions.IneffectiveDictionary.ContainsKey(o.EventType))
                        )
                        .Any(k => endTime >= k.StartTime && startTime <= k.EndTime))
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
}