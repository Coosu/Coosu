using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coosu.Storyboard.Events;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Management
{
    public class ElementCompressor : IDisposable
    {
        public EventHandler<CompressorEventArgs> OperationStart;
        public EventHandler<CompressorEventArgs> OperationEnd;
        public EventHandler<ProcessErrorEventArgs> ErrorOccured; //lock
        //public EventHandler<SituationEventArgs> ElementFound; //lock
        public EventHandler<SituationEventArgs> SituationFound; //lock
        public EventHandler<SituationEventArgs> SituationChanged; //lock
        public EventHandler<ProgressEventArgs> ProgressChanged;

        private int _threadCount = 1;
        private int _pauseThreadCount = 0;

        private readonly ICollection<Sprite> _elements;

        private object _runLock = new object();
        private object _pauseThreadLock = new object();
        private object _situationFoundLock = new object();
        private object _situationChangedLock = new object();

        private CancellationTokenSource _cancelToken;

        public Guid Guid { get; } = Guid.NewGuid();

        public ElementCompressor(ICollection<Sprite> elements)
        {
            this._elements = elements;
        }

        public ElementCompressor(VirtualLayer virtualLayer)
        {
            _elements = virtualLayer.Elements.Where(k => k is Sprite).Cast<Sprite>().ToList();
        }

        public string BackgroundPath { get; set; }

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

            Task[] tasks = RunDequeueTasks(queue, emptyToken);

            RunEnqueueTask(queue, emptyToken);

            await Task.WhenAll(tasks);

            lock (_runLock)
            {
                IsRunning = false;
                OperationEnd?.Invoke(this, new CompressorEventArgs(Guid));
            }
        }

        public async Task CancelTask()
        {
            _cancelToken.Cancel();
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

            _runLock = null;
            _pauseThreadLock = null;
            _cancelToken?.Dispose();

            _elements?.Clear();
        }

        private void RunEnqueueTask(ConcurrentQueue<Sprite> queue, CancellationTokenSource emptyToken)
        {
            var enqueueTask = new Task(() =>
            {
                foreach (var element in _elements)
                {
                    if (_cancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                    queue.Enqueue(element);
                }

                while (!queue.IsEmpty && !_cancelToken.IsCancellationRequested)
                {
                    Thread.Sleep(1);
                }

                emptyToken.Cancel();
            }, emptyToken.Token);

            enqueueTask.Start();
        }

        private Task[] RunDequeueTasks(ConcurrentQueue<Sprite> queue, CancellationTokenSource emptyToken)
        {
            var tasks = new Task[ThreadCount];
            object indexLock = new object();
            int index = 0; //
            int total = _elements.Count();
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    while (!emptyToken.IsCancellationRequested && !_cancelToken.IsCancellationRequested)
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

                        InnerCompress(sprite);

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

        private void InnerCompress(Sprite sprite)
        {
            if (sprite.ImagePath == BackgroundPath &&
                sprite.LayerType == LayerType.Background)
            {
                sprite.IsBackground = true;
            }

            // 每个类型压缩从后往前
            // 1.删除没用的
            // 2.整合能整合的
            // 3.考虑单event情况
            // 4.排除第一行误加的情况 (defaultParams)
            var errorList = new List<string>();

            sprite.OnErrorOccurred += (sender, args) =>
            {
                errorList.Add(args.Message);
            };

            sprite.Examine();
            sprite.OnErrorOccurred = null;

            if (errorList.Count > 0)
            {
                var arg = new ProcessErrorEventArgs
                {
                    Message = $"{sprite.RowInSource} - Examine failed. Found {errorList.Count} error(s):\r\n" +
                              string.Join("\r\n", errorList)
                };

                lock (_pauseThreadLock)
                {
                    if (_cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    _pauseThreadCount++;
                    ErrorOccured?.Invoke(sprite, arg);
                    _pauseThreadCount--;
                }

                if (!arg.Continue)
                    return;
            }

            sprite.FillObsoleteList();
            PreOptimize(sprite);
            NormalOptimize(sprite);
        }

        /// <summary>
        /// 预压缩
        /// </summary>
        private void PreOptimize(EventContainer container)
        {
            if (container is Sprite ele)
            {
                foreach (var item in ele.LoopList)
                {
                    PreOptimize(item);
                }

                foreach (var item in ele.TriggerList)
                {
                    PreOptimize(item);
                }
            }

            if (container.EventList.Any())
                RemoveByObsoletedList(container, container.EventList.ToList());
        }

        /// <summary>
        /// 正常压缩
        /// </summary>
        private void NormalOptimize(EventContainer container)
        {
            if (container is Sprite ele)
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

            if (container.EventList.Any())
            {
                RemoveByLogic(container, container.EventList.ToList());
            }
        }

        /// <summary>
        /// 根据ObsoletedList，移除不必要的命令。
        /// </summary>
        private void RemoveByObsoletedList(EventContainer container, List<CommonEvent> eventList)
        {
            if (container.ObsoleteList.TimingList.Count == 0) return;
            var groups = eventList.GroupBy(k => k.EventType);
            foreach (var group in groups)
            {
                var list = group.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    CommonEvent nowE = list[i];
                    CommonEvent nextE =
                        i == list.Count - 1
                            ? null
                            : list[i + 1];

                    /*
                     * 若当前Event在某Obsolete Range内，且下一Event的StartTime也在此Obsolete Range内，则删除。
                     * 若当前Event是此种类最后一个（无下一个Event），那么需要此Event在某Obsolete Range内，且此Obsolete Range持续到Container结束。
                     * 另注意：若此Event为控制Obsolete Range的Event，则将其过滤。（判断是否正好在某段Obsolete Range的StartTime或EndTime上）
                    */

                    // 判断是否此Event为控制Obsolete Range的Event。
                    if (!(nowE.OnObsoleteTimingRange(container) &&
                          EventExtension.UnworthyDictionary.ContainsKey(nowE.EventType)))
                    {
                        bool canRemove;

                        // 若当前Event是此种类最后一个（无下一个Event)。
                        if (nextE == null)
                        {
                            // 判断是否此Event在某Obsolete Range内，且此Obsolete Range持续到Container结束。
                            canRemove = nowE.InObsoleteTimingRange(container, out var range) &&
                                        range.EndTime == container.MaxTime;

                            if (canRemove)
                            {
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
                                            RaiseSituationEvent(container, SituationType.ThisLastSingleInLastObsoleteToFixTail,
                                                () => { nowE.End[j] = nowE.Start[j]; },
                                                nowE);
                                        }
                                    }

                                    if (nowE.IsSmallerThenMaxTime(container))
                                    {
                                        RaiseSituationEvent(container, SituationType.ThisLastSingleInLastObsoleteToFixEndTime,
                                            () => { nowE.EndTime = nowE.StartTime; },
                                            nowE);
                                    }
                                }
                                else
                                {
                                    RaiseSituationEvent(container, SituationType.ThisLastInLastObsoleteToRemove,
                                        () =>
                                        {
                                            RemoveEvent(container, list, nowE);
                                            i--;
                                        },
                                        nowE);
                                }
                            }

                        } // 若当前Event是此种类最后一个（无下一个Event)。
                        else // 若有下一个Event。
                        {
                            // 判断是否此Event在某Obsolete Range内，且下一Event的StartTime也在此Obsolete Range内。
                            canRemove = container.ObsoleteList.ContainsTimingPoint(out _,
                                nowE.StartTime, nowE.EndTime, nextE.StartTime);

                            if (canRemove)
                            {
                                RaiseSituationEvent(container, SituationType.NextHeadAndThisInObsoleteToRemove,
                                    () =>
                                    {
                                        RemoveEvent(container, list, nowE);
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
        /// <param name="container"></param>
        /// <param name="eventList"></param>
        private void RemoveByLogic(EventContainer container, List<CommonEvent> eventList)
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

                    if (container is Sprite ele &&
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
                        if (nowE.IsTimeInRange(container) && nowE.IsStaticAndDefault() &&
                            list.Count == 1)
                        {
                            RaiseSituationEvent(container, SituationType.ThisFirstSingleIsStaticAndDefaultToRemove,
                                () => { RemoveEvent(container, list, nowE); },
                                nowE);
                        }
                        // 不唯一时
                        else if (nowE.IsTimeInRange(container) && nowE.IsStatic() &&
                            list.Count > 1)
                        {
                            var nextE = list[1];
                            if (EventCompare.IsEventSequent(nowE, nextE))
                            {
                                RaiseSituationEvent(container, SituationType.ThisFirstIsStaticAndSequentWithNextHeadToRemove,
                                    () => { RemoveEvent(container, list, nowE); },
                                    nowE);
                            }
                        }
                        // 当 此event为move，param固定，且唯一时
                        else if (type == EventTypes.Move
                                 && container is Sprite element)
                        {
                            if (list.Count == 1 && nowE.IsStatic()
                                                && nowE.IsTimeInRange(container)
                                                && eventList.Count > 1)
                            {
                                var move = (Move)nowE;
                                if (nowE.Start.All(k => k == (int)k)) //若为小数，不归并
                                {
                                    RaiseSituationEvent(container,
                                        SituationType.MoveSingleIsStaticToRemoveAndChangeInitial,
                                        () =>
                                        {
                                            element.DefaultX = move.StartX;
                                            element.DefaultY = move.StartY;

                                            RemoveEvent(container, list, nowE);
                                        },
                                        nowE);
                                }
                                else if (move.EqualsInitialPosition(element))
                                {
                                    RaiseSituationEvent(container, SituationType.MoveSingleEqualsInitialToRemove,
                                        () => { RemoveEvent(container, list, nowE); },
                                        nowE);
                                }
                                else
                                {
                                    if (element.DefaultX != 0 || element.DefaultY != 0)
                                    {
                                        RaiseSituationEvent(container, SituationType.InitialToZero,
                                            () =>
                                            {
                                                element.DefaultX = 0;
                                                element.DefaultY = 0;
                                            },
                                            nowE);
                                    }
                                }
                            }
                            else
                            {
                                if (element.DefaultX != 0 || element.DefaultY != 0)
                                {
                                    RaiseSituationEvent(container, SituationType.InitialToZero,
                                        () =>
                                        {
                                            element.DefaultX = 0;
                                            element.DefaultY = 0;
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
                        //if (container is Element ele2 &&
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
                            RaiseSituationEvent(container, SituationType.ThisPrevIsStaticAndSequentToCombine,
                                () =>
                                {
                                    preE.EndTime = nowE.EndTime; // 整合至前面: 前一个命令的结束时间延伸

                                    RemoveEvent(container, list, nowE);
                                    index--;
                                },
                                preE, nowE);
                        }
                        /*
                         * 当 此event结束时间 < obj最大时间 (或包括此event有两个以上的最大时间)
                         * 且 此event的param固定
                         * 且 此event当前动作 = 此event上个动作
                        */
                        else if (nowE.IsSmallerThenMaxTime(container) /*||
                                 type == EventTypes.Fade && nowStartP.SequenceEqual(EventExtension.UnworthyDictionary[EventTypes.Fade]) */
                                 && nowE.IsStatic()
                                 && EventCompare.IsEventSequent(preE, nowE))
                        {
                            RaiseSituationEvent(container, SituationType.ThisIsStaticAndSequentWithPrevToCombine,
                                () =>
                                {
                                    RemoveEvent(container, list, nowE);
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
                        else if (nowE.StartTime == preE.EndTime &&
                                 preE.StartTime == preE.EndTime)
                        {
                            if (index > 1 ||
                                preE.EqualsMultiMinTime(container) ||
                                preE.IsStatic() && EventCompare.IsEventSequent(preE, nowE))
                            {
                                RaiseSituationEvent(container, SituationType.PrevIsStaticAndTimeOverlapWithThisStartTimeToRemove,
                                    () =>
                                    {
                                        RemoveEvent(container, list, preE);
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

        private static void RemoveEvent(EventContainer sourceContainer, ICollection<CommonEvent> eventList, CommonEvent e)
        {
            sourceContainer.EventList.Remove(e);
            eventList.Remove(e);
        }

        private void RaiseSituationEvent(EventContainer container, SituationType situationType, Action continueAction, params CommonEvent[] events)
        {
            RaiseSituationEvent(container, situationType, continueAction, null, events);
        }

        private void RaiseSituationEvent(EventContainer container, SituationType situationType, Action continueAction, Action breakAction, params CommonEvent[] events)
        {
            var args = new SituationEventArgs(Guid, situationType)
            {
                Sprite = container is Sprite e ? e : container.BaseElement,
                Container = container is Sprite ? null : container,
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