using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Extensions.Optimizing
{
    public class SpriteCompressor : IDisposable
    {
        public EventHandler<CompressorEventArgs>? OperationStart;
        public EventHandler<CompressorEventArgs>? OperationEnd;
        public EventHandler<ProcessErrorEventArgs>? ErrorOccured; //lock
        //public EventHandler<SituationEventArgs> ElementFound; //lock
        public EventHandler<SituationEventArgs>? SituationFound; //lock
        public EventHandler<SituationEventArgs>? SituationChanged; //lock
        public EventHandler<ProgressEventArgs>? ProgressChanged;

        private int _threadCount = 1;

        private readonly ICollection<Sprite> _sprites;

        private readonly object _runLock = new();
        private readonly object _pauseThreadLock = new();
        private readonly object _situationFoundLock = new();
        private readonly object _situationChangedLock = new();

        private CancellationTokenSource? _cancelToken;
        private readonly ICollection<ISceneObject> _sourceSprites;

        public Guid Guid { get; } = Guid.NewGuid();

        public SpriteCompressor(ICollection<ISceneObject> sprites)
        {
            _sprites = sprites
                .Where(k => k is Sprite)
                .Cast<Sprite>()
                .ToList();
            _sourceSprites = sprites;
        }

        public SpriteCompressor(Layer layer)
        {
            _sprites = layer.SceneObjects
                .Where(k => k is Sprite)
                .Cast<Sprite>()
                .ToList();
            _sourceSprites = layer.SceneObjects;
        }

        public int ThreadCount
        {
            get => _threadCount;
            set
            {
                lock (_runLock) if (IsRunning) throw new Exception();
                _threadCount = value < 1 ? 1 : value;
            }
        }

        public bool IsRunning { get; private set; }

        public async Task<bool> CompressAsync()
        {
            lock (_runLock)
            {
                if (IsRunning)
                {
                    throw new Exception("What");
                }

                IsRunning = true;
                OperationStart?.Invoke(this, new CompressorEventArgs(Guid));
            }

            _cancelToken = new CancellationTokenSource();
            var uselessSprites = new ConcurrentBag<Sprite>();
            var possibleBgs = new ConcurrentBag<Sprite>();

            var isCancel = await RunPTask(uselessSprites, possibleBgs);

            if (isCancel) return false;
            foreach (var uselessSprite in uselessSprites)
            {
                _sourceSprites.Remove(uselessSprite);
            }

            foreach (var grouping in possibleBgs.GroupBy(k => k.ImagePath))
            {
                var imgList = grouping.ToList();
                if (imgList.Count == 1) continue;

                var uselessList = imgList.Where(sprite => !sprite.HasEffectiveTiming()).ToList();
                List<Sprite> removeList = uselessList.Count == imgList.Count
                    ? uselessList.OrderBy(k => k.ToScriptString().Length).Skip(1).ToList()
                    : uselessList;

                foreach (var sprite in removeList)
                {
                    _sourceSprites.Remove(sprite);
                }
            }

            lock (_runLock)
            {
                IsRunning = false;
                OperationEnd?.Invoke(this, new CompressorEventArgs(Guid));
            }

            return true;
        }

        private Task<bool> RunPTask(ConcurrentBag<Sprite> uselessSprites, ConcurrentBag<Sprite> possibleBgs)
        {
            return Task.Factory.StartNew(() =>
            {
                object indexLock = new();
                int index = 0;
                int total = _sprites.Count;
                var mrs = new ManualResetEventSlim(true);
                try
                {
                    _sprites
                        .AsParallel()
                        .WithCancellation(_cancelToken?.Token ?? CancellationToken.None)
                        .WithDegreeOfParallelism(_threadCount)
                        .ForAll(sprite =>
                        {
                            mrs.Wait();

                            var preserve = InnerCompress(mrs, sprite, possibleBgs);
                            if (!preserve) uselessSprites.Add(sprite);
                            lock (indexLock)
                            {
                                index++;
                                ProgressChanged?.Invoke(this, new ProgressEventArgs(Guid)
                                {
                                    Progress = index,
                                    TotalCount = total
                                });
                            }
                        });
                    return false;
                }
                catch (OperationCanceledException)
                {
                    return true;
                }
                finally
                {
                    mrs.Dispose();
                }
            }, TaskCreationOptions.LongRunning);
        }

        public async Task CancelTask()
        {
            _cancelToken?.Cancel();
            await Task.Run(() =>
            {
                while (IsRunning)
                {
                    Thread.Sleep(1);
                }
            });
        }

        public void Dispose()
        {
            ErrorOccured = null;
            ProgressChanged = null;

            //_runLock = null;
            //_pauseThreadLock = null;
            _cancelToken?.Dispose();

            _sprites?.Clear();
        }


        #region Compress Logic

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mrs"></param>
        /// <param name="sprite"></param>
        /// <param name="possibleBgs"></param>
        /// <returns>determine if preserve the object.</returns>
        private bool InnerCompress(ManualResetEventSlim mrs, Sprite sprite, ConcurrentBag<Sprite> possibleBgs)
        {
            // 每个类型压缩从后往前
            // 0.标准化
            // 1.删除没用的
            // 2.整合能整合的
            // 3.考虑单event情况
            // 4.排除第一行误加的情况 (defaultParams)
            var errorList = new List<string?>();

            sprite.Examine((o, e) =>
            {
                errorList.Add(e.Message);
            });

            if (errorList.Count > 0)
            {
                var arg = new ProcessErrorEventArgs(sprite)
                {
                    Message = $"{sprite.RowInSource} - Examine failed. Found {errorList.Count} error(s):\r\n" +
                              string.Join("\r\n", errorList)
                };

                lock (_pauseThreadLock)
                {
                    if (_cancelToken?.IsCancellationRequested != false)
                    {
                        return true;
                    }

                    mrs.Reset();
                    ErrorOccured?.Invoke(sprite, arg);
                    mrs.Set();
                }

                if (!arg.Continue)
                {
                    if (!sprite.HasEffectiveTiming()) return false;
                    return true;
                }
            }

            sprite.Events = new HashSet<ICommonEvent>(sprite.Events);
            StandardizeEvents(sprite);
            var (obsoleteList, controlNodes) = sprite.GetObsoleteList();
            PreOptimize(sprite, obsoleteList, controlNodes);
            NormalOptimize(sprite);
            sprite.Events =
                new SortedSet<ICommonEvent>(sprite.Events, new EventTimingComparer());
            if (sprite.ObjectType == ObjectTypes.Sprite &&
                Path.GetFileName(sprite.ImagePath) == sprite.ImagePath &&
                sprite.LayerType == LayerType.Background)
            {
                possibleBgs.Add(sprite);
                return true;
            }

            return sprite.HasEffectiveTiming();
        }

        private static void StandardizeEvents(Sprite sprite)
        {
            DiscretizeNonStandardEasing(sprite);
            ComputeRelativeEvents(sprite);
        }

        /// <summary>
        /// Make non-standard easing discretized.
        /// </summary>
        /// <param name="sprite"></param>
        private static void DiscretizeNonStandardEasing(IEventHost sprite)
        {
            var commonEvents = sprite.Events.ToList();
            foreach (var @event in commonEvents
                .Where(k => k is not RelativeEvent && k.Easing.TryGetEasingType() == null))
            {
                var eventList = @event.ComputeDiscretizedEvents(false);
                sprite.Events.Remove(@event);
                foreach (var commonEvent in eventList)
                {
                    sprite.Events.Add(commonEvent);
                }
            }
        }

        private static void ComputeRelativeEvents(Sprite sprite)
        {
            var commonEvents = sprite.Events.ToList();
            foreach (var grouping in commonEvents
                    .Where(k => k is RelativeEvent)
                    .Cast<RelativeEvent>().GroupBy(k => k.EventType))
            // todo: discretize and compute all at once to improve performance
            {
                var targetStdType = EventTypes.GetValue(grouping.Key.Index - 100);

                foreach (var @event in grouping)
                {
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
                    if (allThisTypeStandardEvents.Count == 0)
                    {
                        var defaultValue = targetStdType.GetDefaultValue(sprite) ?? throw new NotSupportedException(
                            targetStdType.Flag + " doesn't have any default value.");
                        sprite.Events.Add(CommonEvent.Create(targetStdType, @event.Easing, startTime, endTime,
                            defaultValue,
                            @event.EventType.ComputeRelative(defaultValue, @event.End)));
                        sprite.Events.Remove(@event);
                    }
                    else if (targetStandardEvents.Count == 0)
                    {
                        var lastValue = sprite.ComputeFrame(targetStdType, startTime, null);
                        sprite.Events.Add(CommonEvent.Create(targetStdType, @event.Easing, startTime, endTime,
                            lastValue,
                            @event.EventType.ComputeRelative(lastValue, @event.End)));
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
                        var discretizedRelative = @event.ComputeDiscretizedEvents(false)
                            .Where(k => !k.Start.SequenceEqual(k.End))
                            .ToDictionary(k => (k.StartTime, k.EndTime), k => k);
                        var discretizingTargetStandardEvents =
                            new Dictionary<(double StartTime, double EndTime), ICommonEvent>();
                        var boundedDiscretizingTargetStandardEvents = new HashSet<ICommonEvent>();
                        foreach (CommonEvent k in targetStandardEvents)
                        {
                            ICommonEvent? first = null;
                            ICommonEvent? last = null;
                            foreach (ICommonEvent discretizedEvent in k.ComputeDiscretizedEvents())
                            {
                                first ??= discretizedEvent;
                                last = discretizedEvent;
                                var valueTuple = (discretizedEvent.StartTime, discretizedEvent.EndTime);
                                discretizingTargetStandardEvents.Add(valueTuple, discretizedEvent);
                            }

                            if (first != null) boundedDiscretizingTargetStandardEvents.Add(first);
                            if (last != null) boundedDiscretizingTargetStandardEvents.Add(last);
                        }

                        var list = new List<ICommonEvent>();
                        foreach (var kvp in discretizedRelative.ToList())
                        {
                            var key = kvp.Key;
                            var relativeEvent = kvp.Value;
                            if (discretizingTargetStandardEvents.TryGetValue(key, out var completelyCoincidentEvent))
                            {
                                // exact coincident
                                var eventType = completelyCoincidentEvent.EventType;
                                var newStart =
                                    eventType.ComputeRelative(completelyCoincidentEvent.Start, relativeEvent.Start, 3);
                                var newEnd =
                                    eventType.ComputeRelative(completelyCoincidentEvent.End, relativeEvent.End, 3);
                                completelyCoincidentEvent.Start = newStart;
                                completelyCoincidentEvent.End = newEnd;
                            }
                            else
                            {
                                var bounded = boundedDiscretizingTargetStandardEvents.FirstOrDefault(k =>
                                    k.StartTime <= kvp.Key.EndTime && k.EndTime >= kvp.Key.StartTime);
                                if (bounded == null)
                                {
                                    // nothing
                                    var lastValue = SpriteExtensions.ComputeFrame(allThisTypeStandardEvents,
                                        kvp.Value.EventType, kvp.Key.StartTime, 3);
                                    var commonEvent = CommonEvent.Create(targetStdType, EasingType.Linear,
                                        kvp.Key.StartTime, kvp.Key.EndTime,
                                        @event.EventType.ComputeRelative(lastValue, kvp.Value.Start, 3),
                                        @event.EventType.ComputeRelative(lastValue, kvp.Value.End, 3));
                                    list.Add(commonEvent);
                                }
                                else
                                {
                                    if (kvp.Key.StartTime < bounded.EndTime && kvp.Key.EndTime > bounded.EndTime)
                                    {
                                        var val = SpriteExtensions.ComputeFrame(allThisTypeStandardEvents,
                                            kvp.Value.EventType, kvp.Key.StartTime, 3);

                                        var computeRelative0 = @event.EventType.ComputeRelative(val, kvp.Value.Start, 3);
                                        var newEvent0 = CommonEvent.Create(targetStdType, EasingType.Linear,
                                            bounded.StartTime, kvp.Key.StartTime,
                                            bounded.End.ToArray(), computeRelative0);
                                        var relativeVal = @event.ComputeFrame(bounded.EndTime, 3);
                                        var computeRelative1 = @event.EventType.ComputeRelative(val, relativeVal, 3);
                                        var newEvent1 = CommonEvent.Create(targetStdType, EasingType.Linear,
                                            kvp.Key.StartTime, bounded.EndTime,
                                            computeRelative0, computeRelative1);
                                        var computeRelative2 = @event.EventType.ComputeRelative(val, kvp.Value.End, 3);
                                        var newEvent2 = CommonEvent.Create(targetStdType, EasingType.Linear,
                                            bounded.EndTime, kvp.Key.EndTime,
                                            computeRelative1, computeRelative2);
                                        list.Add(newEvent0);
                                        list.Add(newEvent1);
                                        list.Add(newEvent2);
                                    }
                                    else if (kvp.Key.EndTime > bounded.StartTime && kvp.Key.StartTime < bounded.StartTime)
                                    {
                                        var val = SpriteExtensions.ComputeFrame(allThisTypeStandardEvents,
                                            kvp.Value.EventType, kvp.Key.EndTime, 3);
                                    }
                                    else if (kvp.Key.EndTime.Equals(bounded.StartTime) ||
                                             kvp.Key.StartTime.Equals(bounded.EndTime))
                                    {
                                    }

                                    //Console.WriteLine(bounded.StartTime + "," + bounded.EndTime + "<->" + kvp.Key);
                                    discretizingTargetStandardEvents.Remove((bounded.StartTime, bounded.EndTime));
                                }
                            }

                            discretizedRelative.Remove(key);
                        }

                        foreach (var commonEvent in list)
                        {
                            sprite.Events.Add(commonEvent);
                        }

                        foreach (var value in discretizingTargetStandardEvents.Values)
                        {
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
            }
        }

        /// <summary>
        /// 预压缩
        /// </summary>
        private void PreOptimize(IEventHost host, TimeRange obsoleteList, HashSet<CommonEvent> controlNodes)
        {
            if (host is Sprite ele)
            {
                foreach (var item in ele.LoopList)
                {
                    PreOptimize(item, obsoleteList, controlNodes);
                }

                foreach (var item in ele.TriggerList)
                {
                    PreOptimize(item, obsoleteList, controlNodes);
                }
            }

            if (host.Events.Any())
                RemoveByObsoletedList(host, obsoleteList, controlNodes);
        }

        /// <summary>
        /// 正常压缩
        /// </summary>
        private void NormalOptimize(IEventHost host)
        {
            if (host is Sprite ele)
            {
                foreach (var item in ele.LoopList)
                {
                    NormalOptimize(item);
                }

                foreach (var item in ele.TriggerList)
                {
                    NormalOptimize(item);
                }
            }

            if (host.Events.Any())
            {
                RemoveByLogic(host, host.Events.Cast<CommonEvent>().ToList());
            }
        }

        /// <summary>
        /// 根据ObsoletedList，移除不必要的命令。
        /// </summary>
        private void RemoveByObsoletedList(IEventHost host, TimeRange obsoleteList, HashSet<CommonEvent> controlNodes)
        {
            if (obsoleteList.TimingList.Count == 0) return;
            var groups = host.Events.GroupBy(k => k.EventType);
            foreach (var group in groups)
            {
                var list = group.Cast<CommonEvent>().ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    CommonEvent nowE = list[i];
                    CommonEvent? nextE =
                        i == list.Count - 1
                            ? null
                            : list[i + 1];

                    /*
                     * 若当前Event在某Obsolete Range内，且下一Event的StartTime也在此Obsolete Range内，则删除。
                     * 若当前Event是此种类最后一个（无下一个Event），那么需要此Event在某Obsolete Range内，且此Obsolete Range持续到EventHost结束。
                     * 另注意：若此Event为控制Obsolete Range的Event，则将其过滤。（判断是否正好在某段Obsolete Range的StartTime或EndTime上）
                    */

                    // 判断是否此Event为控制Obsolete Range的Event。
                    if (!(nowE.OnObsoleteTimingRange(obsoleteList) &&
                          EventExtensions.IneffectiveDictionary.ContainsKey(nowE.EventType)))
                    {
                        bool canRemove;

                        // 若当前Event是此种类最后一个（无下一个Event)。
                        if (nextE == null)
                        {
                            // 判断是否此Event在某Obsolete Range内，且此Obsolete Range持续到EventHost结束。
                            canRemove = nowE.InObsoleteTimingRange(obsoleteList, out var range) &&
                                        range.EndTime.Equals(host.MaxTime);
                            if (!canRemove) continue;

                            // M,1,86792,87127,350.1588,489.5946,350.1588,339.5946
                            // M,2,87881,87965,343.8464,333.9836,350.1588,339.5946
                            // F,0,87127,,1
                            // F,0,87713,,0
                            // R,2,87881,87965,-0.04208252,0

                            // R should be kept, but can be optimized
                            if (list.Count == 1)
                            {
                                for (var j = 0; j < nowE.End.Length; j++)
                                {
                                    if (nowE.End[j].ToIcString().Length > nowE.Start[j].ToIcString().Length)
                                    {
                                        RaiseSituationEvent(host, SituationType.ThisLastSingleInLastObsoleteToFixTail,
                                            () => { nowE.End[j] = nowE.Start[j]; },
                                            nowE);
                                    }
                                }

                                if (nowE.IsSmallerThenMaxTime(host))
                                {
                                    RaiseSituationEvent(host, SituationType.ThisLastSingleInLastObsoleteToFixEndTime,
                                        () => { nowE.EndTime = nowE.StartTime; },
                                        nowE);
                                }
                            }
                            else
                            {
                                RaiseSituationEvent(host, SituationType.ThisLastInLastObsoleteToRemove,
                                    () =>
                                    {
                                        RemoveEvent(host, list, nowE);
                                        i--;
                                    },
                                    nowE);
                            }

                        } // 若当前Event是此种类最后一个（无下一个Event)。
                        else // 若有下一个Event。
                        {
                            // 判断是否此Event在某Obsolete Range内，且下一Event的StartTime也在此Obsolete Range内。
                            canRemove = obsoleteList.ContainsTimingPoint(out _,
                                nowE.StartTime, nowE.EndTime, nextE.StartTime) /*&& !controlNodes.Contains(nowE)*/;
                            //目前限制：多个fadoutlist的控制节点不能删的只剩一个
                            if (canRemove)
                            {
                                RaiseSituationEvent(host, SituationType.NextHeadAndThisInObsoleteToRemove,
                                    () =>
                                    {
                                        RemoveEvent(host, list, nowE);
                                        i--;
                                    },
                                    nowE, nextE);
                            }
                        } // 若有下一个Event。
                    } // 判断是否此Event为控制Obsolete Range的Event。
                } // list的循环
            } // group的循环
        }

        /// <summary>
        /// 根据逻辑，进行命令优化。
        /// </summary>
        /// <param name="host"></param>
        /// <param name="eventList"></param>
        private void RemoveByLogic(IEventHost host, List<CommonEvent> eventList)
        {
            var groups = eventList.GroupBy(k => k.EventType);
            foreach (var group in groups)
            {
                EventType type = group.Key;
                var list = group.ToList();

                int index = list.Count - 1;
                while (index >= 0)
                {
                    CommonEvent nowE = list[index];
                    if (host is Sprite ele &&
                        ele.TriggerList.Any(k => nowE.EndTime >= k.StartTime && nowE.StartTime <= k.EndTime) &&
                        ele.LoopList.Any(k => nowE.EndTime >= k.StartTime && nowE.StartTime <= k.EndTime))
                    {
                        index--;
                        continue;
                    }
                    // 若是首个event
                    if (index == 0)
                    {
                        //如果总计只剩一条命令了，不处理
                        if (eventList.Count <= 1) return;

                        //S,0,300,,1
                        //S,0,400,500,0.5
                        /* 
                         * 当 此event结束时间 < obj最大时间 (或包括此event有两个以上的最大时间)
                         * 且 此event开始时间 > obj最小时间 (或包括此event有两个以上的最小时间)
                         * 且 此event的param固定
                         * 且 此event.param=default
                         */

                        // 唯一时
                        if (nowE.IsTimeInRange(host) && nowE.IsStaticAndDefault() &&
                            list.Count == 1)
                        {
                            RaiseSituationEvent(host, SituationType.ThisFirstSingleIsStaticAndDefaultToRemove,
                                () => { RemoveEvent(host, list, nowE); },
                                nowE);
                        }
                        // 不唯一时
                        else if (nowE.IsTimeInRange(host) && nowE.IsStatic() &&
                            list.Count > 1)
                        {
                            var nextE = list[1];
                            if (EventCompare.IsEventSequent(nowE, nextE))
                            {
                                RaiseSituationEvent(host, SituationType.ThisFirstIsStaticAndSequentWithNextHeadToRemove,
                                    () => { RemoveEvent(host, list, nowE); },
                                    nowE);
                            }
                        }
                        // 当 此event为move，param固定，且唯一时
                        else if (type == EventTypes.Move
                                 && host is Sprite sprite)
                        {
                            if (list.Count == 1 && nowE.IsStatic()
                                                && nowE.IsTimeInRange(host)
                                                && eventList.Count > 1)
                            {
                                var move = (Move)nowE;
                                if (nowE.Start.All(k => k.Equals((int)k))) //若为小数，不归并
                                {
                                    RaiseSituationEvent(host,
                                        SituationType.MoveSingleIsStaticToRemoveAndChangeInitial,
                                        () =>
                                        {
                                            sprite.DefaultX = move.StartX;
                                            sprite.DefaultY = move.StartY;

                                            RemoveEvent(host, list, nowE);
                                        },
                                        nowE);
                                }
                                else if (move.EqualsInitialPosition(sprite))
                                {
                                    RaiseSituationEvent(host, SituationType.MoveSingleEqualsInitialToRemove,
                                        () => { RemoveEvent(host, list, nowE); },
                                        nowE);
                                }
                                else
                                {
                                    if (sprite.DefaultX != 0 || sprite.DefaultY != 0)
                                    {
                                        RaiseSituationEvent(host, SituationType.InitialToZero,
                                            () =>
                                            {
                                                sprite.DefaultX = 0;
                                                sprite.DefaultY = 0;
                                            },
                                            nowE);
                                    }
                                }
                            }
                            else
                            {
                                if (sprite.DefaultX != 0 || sprite.DefaultY != 0)
                                {
                                    RaiseSituationEvent(host, SituationType.InitialToZero,
                                        () =>
                                        {
                                            sprite.DefaultX = 0;
                                            sprite.DefaultY = 0;
                                        },
                                        nowE);
                                }
                            }
                        }

                        break;
                    } // 若是首个event
                    else // 若不是首个event
                    {
                        CommonEvent preE = list[index - 1];
                        //if (host is Element ele2 &&
                        //    ele2.TriggerList.Any(k => preE.EndTime >= k.StartTime && preE.StartTime <= k.EndTime) &&
                        //    ele2.LoopList.Any(k => preE.EndTime >= k.StartTime && preE.StartTime <= k.EndTime))
                        //{
                        //    index--;
                        //    continue;
                        //}
                        // 优先进行合并，若不符合再进行删除。
                        /*
                         * 当 此event与前event一致，且前后param皆固定
                        */
                        if (nowE.IsStatic()
                            && preE.IsStatic()
                            && EventCompare.IsEventSequent(preE, nowE))
                        {
                            RaiseSituationEvent(host, SituationType.ThisPrevIsStaticAndSequentToCombine,
                                () =>
                                {
                                    preE.EndTime = nowE.EndTime; // 整合至前面: 前一个命令的结束时间延伸

                                    RemoveEvent(host, list, nowE);
                                    index--;
                                },
                                preE, nowE);
                        }
                        /*
                         * 当 此event结束时间 < obj最大时间 (或包括此event有两个以上的最大时间)
                         * 且 此event的param固定
                         * 且 此event当前动作 = 此event上个动作
                        */
                        else if (nowE.IsSmallerThenMaxTime(host) /*||
                                 type == EventTypes.Fade && nowStartP.SequenceEqual(EventExtension.IneffectiveDictionary[EventTypes.Fade]) */
                                 && nowE.IsStatic()
                                 && EventCompare.IsEventSequent(preE, nowE))
                        {
                            RaiseSituationEvent(host, SituationType.ThisIsStaticAndSequentWithPrevToCombine,
                                () =>
                                {
                                    RemoveEvent(host, list, nowE);
                                    index--;
                                },
                                preE, nowE);
                        }
                        // 存在一种非正常的无效情况，例如：
                        // F,0,0,,0
                        // F,0,0,5000,1
                        // S,0,0,,0.5,0.8
                        // 此时，第一行的F可被删除。或者：
                        // F,0,0,,1
                        // F,0,1000,,0
                        // F,0,1000,5000,1
                        // S,0,0,,0.5,0.8
                        // 此时，第二行的F可被删除。
                        else if (nowE.StartTime.Equals(preE.EndTime) &&
                                 preE.StartTime.Equals(preE.EndTime))
                        {
                            if (index > 1 ||
                                preE.EqualsMultiMinTime(host) ||
                                preE.IsStatic() && EventCompare.IsEventSequent(preE, nowE))
                            {
                                RaiseSituationEvent(host, SituationType.PrevIsStaticAndTimeOverlapWithThisStartTimeToRemove,
                                    () =>
                                    {
                                        RemoveEvent(host, list, preE);
                                        index--;
                                    },
                                    preE, nowE);
                            }
                            else
                                index--;
                        }
                        else index--;
                    } // 若不是首个event
                } // list的循环
            } // group的循环
        }

        private static void RemoveEvent(IEventHost sourceHost, ICollection<CommonEvent> eventList, CommonEvent e)
        {
            var success = sourceHost.Events.Remove(e);
            if (!success)
            {
                //var count = sourceHost.Events.RemoveWhere(k =>
                //    k.Start.SequenceEqual(e.Start) &&
                //    k.End.SequenceEqual(e.End) &&
                //    k.StartTime.Equals(e.StartTime) &&
                //    k.EndTime.Equals(e.EndTime));
                //if (count <= 0)
                //{
                //    throw new Exception("Failed to delete event");
                //}
            }
            eventList.Remove(e);
        }

        private void RaiseSituationEvent(IEventHost host, SituationType situationType, Action continueAction, params ICommonEvent[] events)
        {
            RaiseSituationEvent(host, situationType, continueAction, null, events);
        }

        private void RaiseSituationEvent(IEventHost host, SituationType situationType, Action continueAction,
            Action? breakAction, params ICommonEvent[] events)
        {
            var sprite = host is ISubEventHost e ? e.BaseObject as Sprite : host as Sprite;
            var args = new SituationEventArgs(Guid, situationType)
            {
                Sprite = sprite,
                Host = host,
                Events = events
            };

            if (SituationFound != null)
            {
                lock (_situationFoundLock)
                {
                    SituationFound?.Invoke(this, args);
                }
            }

            if (args.Continue)
            {
                continueAction?.Invoke();

                if (SituationChanged != null)
                {
                    lock (_situationChangedLock)
                    {
                        SituationChanged?.Invoke(this, args);
                    }
                }
            }
            else
                breakAction?.Invoke();
        }

        #endregion Compress Logic
    }
}