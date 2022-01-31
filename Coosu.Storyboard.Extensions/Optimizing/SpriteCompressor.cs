using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coosu.Shared;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Extensions.Computing;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Extensions.Optimizing
{
    public class SpriteCompressor : IDisposable
    {
        private readonly Layer? _layer;
        public CompressOptions Options { get; }
        public event EventHandler<CompressorEventArgs>? OperationStart;
        public event EventHandler<CompressorEventArgs>? OperationEnd;
        public event EventHandler<ProcessErrorEventArgs>? ErrorOccured; //lock
        //public EventHandler<SituationEventArgs> ElementFound; //lock
        public event AsyncEventHandler<SituationEventArgs>? SituationFound; //lock
        public event AsyncEventHandler<SituationEventArgs>? SituationChanged; //lock
        public event EventHandler<ProgressEventArgs>? ProgressChanged;

        //private int _threadCount = 1;

        private ICollection<Sprite> _targetSprites;

        private readonly object _runLock = new();
        private readonly object _pauseThreadLock = new();
        private readonly SemaphoreSlim _situationFoundLock = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _situationChangedLock = new SemaphoreSlim(1, 1);

        private CancellationTokenSource? _cancelToken;
        private readonly ICollection<ISceneObject> _sourceSprites;

        public Guid Guid { get; } = Guid.NewGuid();

        public SpriteCompressor(ICollection<ISceneObject> sprites, CompressOptions? compressSettings = null)
        {
            _targetSprites = sprites
                .Where(k => k is Sprite)
                .Cast<Sprite>()
                .ToList();
            _sourceSprites = sprites;
            Options = compressSettings ?? new CompressOptions();
        }

        public SpriteCompressor(Layer layer, CompressOptions? compressSettings = null)
        {
            _layer = layer;
            _targetSprites = layer.SceneObjects
                .Where(k => k is Sprite)
                .Cast<Sprite>()
                .ToList();
            _sourceSprites = layer.SceneObjects;
            Options = compressSettings ?? new CompressOptions();
        }

        //public int ThreadCount
        //{
        //    get => _threadCount;
        //    set
        //    {
        //        lock (_runLock) if (IsRunning) throw new Exception();
        //        _threadCount = value < 1 ? 1 : value;
        //    }
        //}

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

            if (_layer != null)
            {
                if (_layer.ExpandSubHosts())
                    _targetSprites = _layer.SceneObjects
                        .Where(k => k is Sprite)
                        .Cast<Sprite>()
                        .ToList();
            }

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
                int total = _targetSprites.Count;
                var mrs = new ManualResetEventSlim(true);
                try
                {
                    _targetSprites
                        .AsParallel()
                        .WithCancellation(_cancelToken?.Token ?? CancellationToken.None)
                        .WithDegreeOfParallelism(Options.ThreadCount)
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

            _targetSprites?.Clear();
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

            // temporary to object equals
            sprite.Events = new HashSet<IKeyEvent>(sprite.Events);

            // relative to absolute
            // todo: performance issue
            sprite.StandardizeEvents(Options.DiscretizingInterval, Options.DiscretizingAccuracy);

            var obsoleteList = sprite.ComputeInvisibleRange(out var keyEvents);
            PreOptimize(sprite, obsoleteList, keyEvents);
            NormalOptimize(sprite);

            // to compute equals
            sprite.Events = new SortedSet<IKeyEvent>(sprite.Events, new EventSequenceComparer());

            if (sprite.ObjectType == ObjectTypes.Sprite &&
                Path.GetFileName(sprite.ImagePath) == sprite.ImagePath &&
                sprite.LayerType == LayerType.Background)
            {
                possibleBgs.Add(sprite);
                return true;
            }

            return sprite.HasEffectiveTiming();
        }

        /// <summary>
        /// 预压缩
        /// </summary>
        private void PreOptimize(IDetailedEventHost host, TimeRange obsoleteList, HashSet<BasicEvent> keyEvents)
        {
            if (host is Sprite ele)
            {
                for (var i = 0; i < ele.LoopList.Count; i++)
                {
                    var item = ele.LoopList[i];
                    PreOptimize(item, obsoleteList, keyEvents);
                }

                for (var i = 0; i < ele.TriggerList.Count; i++)
                {
                    var item = ele.TriggerList[i];
                    PreOptimize(item, obsoleteList, keyEvents);
                }
            }

            if (host.Events.Count > 0)
                RemoveByInvisibleList(host, obsoleteList, keyEvents);

            foreach (var hostEvent in host.Events)
            {
                if (hostEvent.EventType.Size < 1) continue;
                if (hostEvent.StartTime.Equals(hostEvent.EndTime) && !hostEvent.IsStartsEqualsEnds())
                {
                    hostEvent.Fill();
                    for (int i = 0; i < hostEvent.EventType.Size; i++)
                    {
                        hostEvent.SetValue(i, hostEvent.GetValue(i + hostEvent.EventType.Size));
                    }
                }
            }
        }

        /// <summary>
        /// 正常压缩
        /// </summary>
        private void NormalOptimize(IDetailedEventHost host)
        {
            if (host is Sprite ele)
            {
                for (var i = 0; i < ele.LoopList.Count; i++)
                {
                    var item = ele.LoopList[i];
                    NormalOptimize(item);
                }

                for (var i = 0; i < ele.TriggerList.Count; i++)
                {
                    var item = ele.TriggerList[i];
                    NormalOptimize(item);
                }
            }

            if (host.Events.Count > 0)
            {
                RemoveByLogic(host, host.Events.Cast<BasicEvent>().ToList());
            }
        }

        /// <summary>
        /// 根据InvisibleList，移除不必要的命令。
        /// </summary>
        private void RemoveByInvisibleList(IDetailedEventHost host, TimeRange obsoleteList, HashSet<BasicEvent> keyEvents)
        {
            if (obsoleteList.TimingList.Count == 0) return;
            var groups = host.Events.GroupBy(k => k.EventType);
            foreach (var group in groups)
            {
                var list = group.Cast<BasicEvent>().ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    BasicEvent nowE = list[i];
                    BasicEvent? nextE =
                        i == list.Count - 1
                            ? null
                            : list[i + 1];

                    /*
                     * 若当前Event在某Invisible Range内，且下一Event的StartTime也在此Invisible Range内，则删除。
                     * 若当前Event是此种类最后一个（无下一个Event），那么需要此Event在某Invisible Range内，且此Invisible Range持续到EventHost结束。
                     * 另注意：若此Event为控制Invisible Range的Event，则将其过滤。（判断是否正好在某段Invisible Range的StartTime或EndTime上）
                    */

                    // 判断是否此Event为控制Invisible Range的Event。
                    if (!(nowE.OnInvisibleTimingRangeBound(obsoleteList) &&
                          EventExtensions.IneffectiveDictionary.ContainsKey(nowE.EventType.Flag)))
                    {
                        bool canRemove;

                        // 若当前Event是此种类最后一个（无下一个Event)。
                        if (nextE == null)
                        {
                            // 判断是否此Event在某Invisible Range内，且此Invisible Range持续到EventHost结束。
                            canRemove = nowE.InInvisibleTimingRange(obsoleteList, out var range) &&
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
                                for (var j = 0; j < nowE.EventType.Size; j++)
                                {
                                    // 好像不需要这个判断，因为两个控制参数和一个比肯定一个短
                                    //if (nowE.GetEndsValue(j).ToIcString().Length > nowE.GetStartsValue(j).ToIcString().Length)
                                    {
                                        RaiseSituationEvent(host, SituationType.ThisLastSingleInLastInvisibleToFixTail,
                                            () => { nowE.SetEndsValue(j, nowE.GetStartsValue(j)); },
                                            nowE);
                                    }
                                }

                                if (nowE.IsSmallerThanMaxTime(host))
                                {
                                    RaiseSituationEvent(host, SituationType.ThisLastSingleInLastInvisibleToFixEndTime,
                                        () => { nowE.EndTime = nowE.StartTime; },
                                        nowE);
                                }
                            }
                            else
                            {
                                RaiseSituationEvent(host, SituationType.ThisLastInLastInvisibleToRemove,
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
                            // 判断是否此Event在某Invisible Range内，且下一Event的StartTime也在此Invisible Range内。
                            canRemove = obsoleteList.ContainsTimingPoint(out _,
                                nowE.StartTime, nowE.EndTime, nextE.StartTime) /*&& !keyEvents.Contains(nowE)*/;
                            // todo: 目前限制：多个fadoutlist的控制节点不能删的只剩一个
                            if (canRemove)
                            {
                                RaiseSituationEvent(host, SituationType.NextHeadAndThisInInvisibleToRemove,
                                    () =>
                                    {
                                        RemoveEvent(host, list, nowE);
                                        i--;
                                    },
                                    nowE, nextE);
                            }
                        } // 若有下一个Event。
                    } // 判断是否此Event为控制Invisible Range的Event。
                } // list的循环
            } // group的循环
        }

        /// <summary>
        /// 根据逻辑，进行命令优化。
        /// </summary>
        /// <param name="host"></param>
        /// <param name="eventList"></param>
        private void RemoveByLogic(IDetailedEventHost host, List<BasicEvent> eventList)
        {
            var groups = eventList.GroupBy(k => k.EventType);
            foreach (var group in groups)
            {
                EventType type = group.Key;
                var list = group.ToList();

                int index = list.Count - 1;
                while (index >= 0)
                {
                    BasicEvent nowE = list[index];
                    if (host is Sprite ele &&
                        ele.TriggerList.Count > 0 && 
                        ele.TriggerList.Any(k => nowE.EndTime >= k.StartTime && nowE.StartTime <= k.EndTime) &&
                        ele.LoopList.Count > 0 &&
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
                        else if (nowE.IsTimeInRange(host) && nowE.IsStartsEqualsEnds() &&
                            list.Count > 1)
                        {
                            var nextE = list[1];
                            if (nowE.SuccessiveTo(nextE))
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
                            if (list.Count == 1 && nowE.IsStartsEqualsEnds()
                                                && nowE.IsTimeInRange(host)
                                                && eventList.Count > 1)
                            {
                                var move = (Move)nowE;
                                if (nowE.GetStarts().All(k => k.Equals((int)k))) //若为小数，不归并
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
                        BasicEvent preE = list[index - 1];
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
                        if (nowE.IsStartsEqualsEnds()
                            && preE.IsStartsEqualsEnds()
                            && preE.SuccessiveTo(nowE))
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
                        else if (nowE.IsSmallerThanMaxTime(host) /*||
                                 type == EventTypes.Fade && nowStartP.SequenceEqual(EventExtension.IneffectiveDictionary[EventTypes.Fade]) */
                                 && nowE.IsStartsEqualsEnds()
                                 && preE.SuccessiveTo(nowE))
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
                                preE.IsStartsEqualsEnds() && preE.SuccessiveTo(nowE))
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

        private static void RemoveEvent(IDetailedEventHost sourceHost, ICollection<BasicEvent> eventList, BasicEvent e)
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

        private async Task RaiseSituationEvent(IDetailedEventHost host, SituationType situationType, Action continueAction, params IKeyEvent[] events)
        {
            await RaiseSituationEvent(host, situationType, continueAction, null, events);
        }

        private async Task RaiseSituationEvent(IDetailedEventHost host, SituationType situationType, Action continueAction,
            Action? breakAction, params IKeyEvent[] events)
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
                await _situationFoundLock.WaitAsync();

                try
                {
                    await SituationFound.Invoke(this, args);
                }
                finally
                {
                    _situationFoundLock.Release();
                }

            }

            if (args.Continue)
            {
                continueAction?.Invoke();

                if (SituationChanged != null)
                {
                    await _situationChangedLock.WaitAsync();

                    try
                    {
                        await SituationChanged.Invoke(this, args);
                    }
                    finally
                    {
                        _situationChangedLock.Release();
                    }
                }
            }
            else
                breakAction?.Invoke();
        }

        #endregion Compress Logic
    }
}