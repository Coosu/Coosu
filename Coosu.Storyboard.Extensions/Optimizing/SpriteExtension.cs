using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Extensions.Optimizing
{
    public static class SpriteExtension
    {
        public static async Task ExpandAsync(this Layer eleG)
        {
            await Task.Run(() => { Expand(eleG); });
        }

        public static void ExpandAndFillFadeout(this Layer eleG)
        {
            eleG.InnerFix(true, true);
        }

        public static void Expand(this Layer eleG)
        {
            eleG.InnerFix(true, false);
        }

        public static void FillObsoleteList(this Layer eleG)
        {
            eleG.InnerFix(false, true);
        }

        public static void Expand(this IEventHost host)
        {
            if (host is Sprite sprite)
            {
                if (sprite.TriggerList.Any())
                {
                    foreach (var t in sprite.TriggerList)
                        t.Expand();
                }

                if (sprite.LoopList.Any())
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
                                    CommonEvent.Create(e.EventType,
                                        e.Easing,
                                        fixedStartTime + e.StartTime, fixedStartTime + e.EndTime,
                                        e.Start, e.End)
                                );
                            }
                        }
                    }

                    sprite.LoopList.Clear();
                }
            }

            var events = host.Events?.GroupBy(k => k.EventType);
            if (events == null) return;
            foreach (var kv in events)
            {
                List<ICommonEvent> list = kv.ToList();
                for (var i = 0; i < list.Count - 1; i++)
                {
                    if (list[i].Start == list[i].End) // case 1
                    {
                        list[i].EndTime = list[i + 1].StartTime;
                    }

                    if (!list[i].EndTime.Equals(list[i + 1].StartTime)) // case 2
                    {
                        host.AddEvent(
                            CommonEvent.Create(list[i].EventType,
                                EasingType.Linear,
                                list[i].EndTime, list[i + 1].StartTime,
                                list[i].End, list[i].End)
                        );
                    }
                }
            }
        }

        public static (TimeRange ObsoleteList, HashSet<ICommonEvent> ControlNode) GetObsoleteList(this Sprite sprite)
        {
            var obsoleteList = new TimeRange();
            var controlNode = new HashSet<ICommonEvent>();
            var possibleList = sprite.Events
                .Where(k => k.EventType == EventTypes.Fade ||
                            k.EventType == EventTypes.Scale ||
                            k.EventType == EventTypes.Vector)
                .ToArray();

            if (possibleList.Length <= 0)
                return (obsoleteList, controlNode);

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
                    e.Start.SequenceEqual(e.GetUnworthyValue()) &&
                    e.StartTime > sprite.MinTime)
                {
                    if (e.IsStatic)
                        e.AdjustTiming(sprite.MinTime - e.StartTime); // todo: stop changing here
                    dic[e.EventType].StartTime = sprite.MinTime;
                    dic[e.EventType].IsFadingOut = true;
                    controlNode.Add(e);
                }

                // event.Start和End都为无用值时，开始计时
                if (e.Start.SequenceEqual(e.GetUnworthyValue()) &&
                    e.End.SequenceEqual(e.GetUnworthyValue()) &&
                    dic[e.EventType].IsFadingOut == false)
                {
                    dic[e.EventType].StartTime = e.StartTime;
                    dic[e.EventType].IsFadingOut = true;
                    controlNode.Add(e);
                }
                // event.End为无用值时，开始计时
                else if (e.End.SequenceEqual(e.GetUnworthyValue()) &&
                         dic[e.EventType].IsFadingOut == false)
                {
                    dic[e.EventType].StartTime = e.EndTime;
                    dic[e.EventType].IsFadingOut = true;
                    controlNode.Add(e);
                }
                else if (dic[e.EventType].IsFadingOut)
                {
                    if (e.Start.SequenceEqual(e.GetUnworthyValue()) &&
                        e.End.SequenceEqual(e.GetUnworthyValue()))
                        continue;
                    AddTimeRage(dic[e.EventType].StartTime, e.StartTime);
                    dic[e.EventType].IsFadingOut = false;
                    dic[e.EventType].StartTime = double.MinValue;
                    controlNode.Add(e);
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
                                .Any(o => EventExtensions.UnworthyDictionary.ContainsKey(o.EventType))
                        )
                        .Any(k => endTime >= k.StartTime && startTime <= k.EndTime)
                    ||
                    sprite.LoopList
                        .Where(k =>
                            k.Events
                                .Any(o => EventExtensions.UnworthyDictionary.ContainsKey(o.EventType))
                        )
                        .Any(k => endTime >= k.StartTime && startTime <= k.EndTime))
                {
                    return;
                }

                obsoleteList.Add(startTime, endTime);
            }

            return (obsoleteList, controlNode);
        }

        /// <summary>
        /// 检查timing是否合法.
        /// </summary>
        public static void Examine(this IEventHost host, EventHandler<ProcessErrorEventArgs>? onError)
        {
            var events = host.Events.Where(k => k is not RelativeEvent).GroupBy(k => k.EventType);
            foreach (var kv in events)
            {
                var list = kv.ToArray();
                for (var i = 0; i < list.Length - 1; i++)
                {
                    ICommonEvent objNext = list[i + 1];
                    ICommonEvent objNow = list[i];
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

        private static void InnerFix(this Layer eleG, bool expand, bool fillFadeout)
        {
            if (!expand && !fillFadeout)
                return;
            foreach (var ec in eleG.SceneObjects)
            {
                if (ec is not Sprite ele) continue;

                if (expand) ele.Expand();
                if (fillFadeout) ele.GetObsoleteList();
            }
        }

        public class EventContext
        {
            public int Count { get; set; } = 0;
            public bool IsFadingOut { get; set; } = false;
            public double StartTime { get; set; } = double.MinValue;
        }
    }
}
