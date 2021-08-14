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
        private int _pauseThreadCount = 0;

        private readonly ICollection<Sprite> _sprites;

        private readonly object _runLock = new();
        private readonly object _pauseThreadLock = new();
        private readonly object _situationFoundLock = new();
        private readonly object _situationChangedLock = new();

        private CancellationTokenSource? _cancelToken;
        private readonly ICollection<ISceneObject> _sourceSprites;

        public Guid Guid { get; } = Guid.NewGuid();

        public SpriteCompressor(ICollection<Sprite> sprites)
        {
            _sprites = sprites;
            _sourceSprites = (ICollection<ISceneObject>)sprites;
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
                lock (_runLock)
                {
                    if (IsRunning)
                        throw new Exception();
                }

                _threadCount =
                    (value < 1
                        ? 1
                        : (value > 4
                            ? 4
                            : value)
                    );
            }
        }

        public bool IsRunning { get; private set; }

        public async Task CompressAsync()
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

            var queue = new ConcurrentQueue<Sprite>();

            var emptyToken = new CancellationTokenSource();
            _cancelToken = new CancellationTokenSource();
            var uselessSprites = new ConcurrentBag<Sprite>();
            var possibleBgs = new ConcurrentBag<Sprite>();
            Task[] tasks = RunDequeueTasks(queue, uselessSprites, possibleBgs, emptyToken);

            RunEnqueueTask(queue, emptyToken);

            await Task.WhenAll(tasks);

            foreach (var uselessSprite in uselessSprites)
            {
                _sourceSprites.Remove(uselessSprite);
            }

            foreach (var grouping in possibleBgs.GroupBy(k => k.ImagePath))
            {
                var imgList = grouping.ToList();
                if (imgList.Count == 1) continue;

                var uselessList = imgList.Where(sprite => !GetSpriteIsValidTiming(sprite)).ToList();
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

        private void RunEnqueueTask(ConcurrentQueue<Sprite> queue, CancellationTokenSource emptyToken)
        {
            var enqueueTask = new Task(() =>
            {
                foreach (var sprite in _sprites)
                {
                    if (_cancelToken?.IsCancellationRequested != false)
                    {
                        break;
                    }

                    queue.Enqueue(sprite);
                }

                while (!queue.IsEmpty && _cancelToken?.IsCancellationRequested != true)
                {
                    Thread.Sleep(1);
                }

                emptyToken.Cancel();
            }, emptyToken.Token);

            enqueueTask.Start();
        }

        private Task[] RunDequeueTasks(ConcurrentQueue<Sprite> queue,
            ConcurrentBag<Sprite> uselessSprites,
            ConcurrentBag<Sprite> possibleBgs,
            CancellationTokenSource emptyToken)
        {
            var tasks = new Task[ThreadCount];
            object indexLock = new object();
            int index = 0; //
            int total = _sprites.Count();
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    while (!emptyToken.IsCancellationRequested && _cancelToken?.IsCancellationRequested != true)
                    {
                        Sprite sprite;

                        if (!queue.IsEmpty)
                        {
                            if (!queue.TryDequeue(out sprite))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }

                        while (_pauseThreadCount != 0)
                        {
                            Thread.Sleep(1);
                        }

                        var preserve = InnerCompress(sprite, possibleBgs);
                        if (!preserve) uselessSprites.Add(sprite);
                        lock (indexLock)
                        {
                            index++;
                        }

                        ProgressChanged?.Invoke(this, new ProgressEventArgs(Guid)
                        {
                            Progress = index,
                            TotalCount = total
                        });
                    }
                }, emptyToken.Token);
            }

            return tasks;
        }

        #region Compress Logic

        private bool InnerCompress(Sprite sprite, ConcurrentBag<Sprite> possibleBgs)
        {
            // 每个类型压缩从后往前
            // 1.删除没用的
            // 2.整合能整合的
            // 3.考虑单event情况
            // 4.排除第一行误加的情况 (defaultParams)
            var errorList = new List<string?>();

            void OnErrorOccured(object sender, ProcessErrorEventArgs args)
            {
                errorList.Add(args.Message);
            }

            ErrorOccured += OnErrorOccured;
            sprite.Examine(ErrorOccured);
            ErrorOccured -= OnErrorOccured;

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

                    _pauseThreadCount++;
                    ErrorOccured?.Invoke(sprite, arg);
                    _pauseThreadCount--;
                }

                if (!arg.Continue)
                {
                    if (!GetSpriteIsValidTiming(sprite)) return false;
                    return true;
                }
            }

            sprite.Events = new HashSet<ICommonEvent>(sprite.Events);
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

            if (!GetSpriteIsValidTiming(sprite))
                return false;

            return true;
        }

        private static bool GetSpriteIsValidTiming(Sprite sprite)
        {
            if (sprite.MaxTime < sprite.MinTime)
                return false;
            if (sprite.MaxTime.Equals(sprite.MinTime))
                return false;
            return true;
        }

        /// <summary>
        /// 预压缩
        /// </summary>
        private void PreOptimize(IEventHost host, TimeRange obsoleteList, HashSet<ICommonEvent> controlNodes)
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
                RemoveByLogic(host, host.Events.ToList());
            }
        }

        /// <summary>
        /// 根据ObsoletedList，移除不必要的命令。
        /// </summary>
        private void RemoveByObsoletedList(IEventHost host, TimeRange obsoleteList, HashSet<ICommonEvent> controlNodes)
        {
            if (obsoleteList.TimingList.Count == 0) return;
            var groups = host.Events.GroupBy(k => k.EventType);
            foreach (var group in groups)
            {
                var list = group.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    ICommonEvent nowE = list[i];
                    ICommonEvent? nextE =
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
                          EventExtensions.UnworthyDictionary.ContainsKey(nowE.EventType)))
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
        private void RemoveByLogic(IEventHost host, List<ICommonEvent> eventList)
        {
            var groups = eventList.GroupBy(k => k.EventType);
            foreach (var group in groups)
            {
                EventType type = group.Key;
                var list = group.ToList();

                int index = list.Count - 1;
                while (index >= 0)
                {
                    ICommonEvent nowE = list[index];
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
                        ICommonEvent preE = list[index - 1];
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
                                 type == EventTypes.Fade && nowStartP.SequenceEqual(EventExtension.UnworthyDictionary[EventTypes.Fade]) */
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

        private static void RemoveEvent(IEventHost sourceHost, ICollection<ICommonEvent> eventList, ICommonEvent e)
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