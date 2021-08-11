using Coosu.Storyboard.Events;
using Coosu.Storyboard.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coosu.Storyboard.Management
{
    public static class ElementExtension
    {
        public static async Task ExpandAsync(this VirtualLayer eleG)
        {
            await Task.Run(() => { Expand(eleG); });
        }

        public static void ExpandAndFillFadeout(this VirtualLayer eleG)
        {
            eleG.InnerFix(true, true);
        }

        public static void Expand(this VirtualLayer eleG)
        {
            eleG.InnerFix(true, false);
        }

        public static void FillObsoleteList(this VirtualLayer eleG)
        {
            eleG.InnerFix(false, true);
        }

        public static void Expand(this EventHost host)
        {
            if (host is Sprite element)
            {
                if (element.TriggerList.Any())
                {
                    foreach (var t in element.TriggerList)
                        t.Expand();
                }

                if (element.LoopList.Any())
                {
                    foreach (var loop in element.LoopList)
                    {
                        loop.Expand();
                        var loopCount = loop.LoopCount;
                        var startTime = loop.StartTime;
                        for (int count = 0; count < loopCount; count++)
                        {
                            var fixedStartTime = startTime + (count * loop.MaxTime);
                            foreach (var e in loop.Events)
                            {
                                element.AddEvent(
                                    e.EventType,
                                    e.Easing,
                                    fixedStartTime + e.StartTime, fixedStartTime + e.EndTime,
                                    e.Start, e.End
                                );
                            }
                        }
                    }

                    element.LoopList.Clear();
                }
            }

            var events = host.Events?.GroupBy(k => k.EventType);
            if (events == null) return;
            foreach (var kv in events)
            {
                List<CommonEvent> list = kv.ToList();
                for (var i = 0; i < list.Count - 1; i++)
                {
                    if (list[i].Start == list[i].End) // case 1
                    {
                        list[i].EndTime = list[i + 1].StartTime;
                    }

                    if (!list[i].EndTime.Equals(list[i + 1].StartTime)) // case 2
                    {
                        host.AddEvent(
                            list[i].EventType,
                            EasingType.Linear,
                            list[i].EndTime, list[i + 1].StartTime,
                            list[i].End, list[i].End
                        );
                    }
                }
            }
        }

        public static void FillObsoleteList(this Sprite sprite)
        {
            var possibleList = sprite.Events
                .Where(k => k.EventType == EventTypes.Fade ||
                            k.EventType == EventTypes.Scale ||
                            k.EventType == EventTypes.Vector);

            if (possibleList.Any())
            {
                var dic = new Dictionary<EventType, EventSettings>
                {
                    [EventTypes.Fade] = new EventSettings(),
                    [EventTypes.Scale] = new EventSettings(),
                    [EventTypes.Vector] = new EventSettings()
                };
                foreach (var e in possibleList)
                {
                    if (e.EventType == EventTypes.Fade)
                    {

                    }
                    dic[e.EventType].Count++;
                    // 最早的event晚于最小开始时间，默认加这一段
                    if (dic[e.EventType].Count == 1 &&
                        e.Start.SequenceEqual(e.GetUnworthyValue()) &&
                        e.StartTime > sprite.MinTime)
                    {
                        dic[e.EventType].StartTime = sprite.MinTime;
                        dic[e.EventType].IsFadingOut = true;
                    }

                    // event.Start和End都为无用值时，开始计时
                    if (e.Start.SequenceEqual(e.GetUnworthyValue()) &&
                        e.End.SequenceEqual(e.GetUnworthyValue()) &&
                        dic[e.EventType].IsFadingOut == false)
                    {
                        dic[e.EventType].StartTime = e.StartTime;
                        dic[e.EventType].IsFadingOut = true;
                    }
                    // event.End为无用值时，开始计时
                    else if (e.End.SequenceEqual(e.GetUnworthyValue()) &&
                        dic[e.EventType].IsFadingOut == false)
                    {
                        dic[e.EventType].StartTime = e.EndTime;
                        dic[e.EventType].IsFadingOut = true;
                    }
                    else if (dic[e.EventType].IsFadingOut)
                    {
                        if (e.Start.SequenceEqual(e.GetUnworthyValue()) &&
                            e.End.SequenceEqual(e.GetUnworthyValue()))
                            continue;
                        AddTimeRage(dic[e.EventType].StartTime, e.StartTime);
                        dic[e.EventType].IsFadingOut = false;
                        dic[e.EventType].StartTime = float.MinValue;
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
            }

            void AddTimeRage(float startTime, float endTime)
            {
                if (sprite.TriggerList
                        .Where(k =>
                            k.Events
                                .Any(o => EventExtension.UnworthyDictionary.ContainsKey(o.EventType))
                        )
                        .Any(k => endTime >= k.StartTime && startTime <= k.EndTime)
                    ||
                    sprite.LoopList
                        .Where(k =>
                            k.Events
                                .Any(o => EventExtension.UnworthyDictionary.ContainsKey(o.EventType))
                        )
                        .Any(k => endTime >= k.StartTime && startTime <= k.EndTime))
                {
                    return;
                }

                sprite.ObsoleteList.Add(startTime, endTime);
            }
        }

        /// <summary>
        /// 检查timing是否合法.
        /// </summary>
        public static void Examine(this EventHost host)
        {
            var events = host.Events.GroupBy(k => k.EventType);
            foreach (var kv in events)
            {
                var list = kv.ToArray();
                for (var i = 0; i < list.Length - 1; i++)
                {
                    CommonEvent objNext = list[i + 1];
                    CommonEvent objNow = list[i];
                    if (objNow.StartTime > objNow.EndTime)
                    {
                        var info = $"{{{objNow}}}:\r\n" +
                                   $"Start time should not be larger than end time.";

                        var arg = new ProcessErrorEventArgs()
                        {
                            Message = info
                        };
                        host.OnErrorOccurred?.Invoke(host, arg);
                        if (!arg.Continue)
                        {
                            return;
                        };
                    }
                    if (objNext.StartTime < objNow.EndTime)
                    {
                        var info = $"{{{objNow}}} to {{{objNext}}}:\r\n" +
                                   $"The previous object's end time should be larger than the next object's start time.";
                        var arg = new ProcessErrorEventArgs
                        {
                            Message = info
                        };
                        host.OnErrorOccurred?.Invoke(host, arg);
                        if (!arg.Continue)
                        {
                            return;
                        }
                    }
                }
            }

            if (!(host is Sprite e))
                return;

            foreach (var item in e.LoopList)
            {
                Examine(item);
            }

            foreach (var item in e.TriggerList)
            {
                Examine(item);
            }
        }

        private static void InnerFix(this VirtualLayer eleG, bool expand, bool fillFadeout)
        {
            if (!expand && !fillFadeout)
                return;
            foreach (var ec in eleG.SceneObjects)
            {
                if (!(ec is Sprite ele)) continue;

                if (expand) ele.Expand();
                if (fillFadeout) ele.FillObsoleteList();
            }
        }

        public class EventSettings
        {
            public int Count { get; set; } = 0;
            public bool IsFadingOut { get; set; } = false;
            public float StartTime { get; set; } = float.MinValue;
        }
    }
}
