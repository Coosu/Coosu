using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Shared;
using Coosu.Shared.Mathematics;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Events
{
    public sealed class Loop : ISubEventHost, IEvent
    {
        public event Action? TimingChanged;
        public EventType EventType { get; } = EventTypes.Loop;

        internal ISceneObject? _baseObject;
        private double _startTime;
        private ICollection<IKeyEvent> _events = new SortedSet<IKeyEvent>(EventSequenceComparer.Instance);

        private double? _cachedMaxTime;
        private double? _cachedMinTime;
        private double? _cachedMaxStartTime;
        private double? _cachedMinEndTime;

        public IReadOnlyCollection<IKeyEvent> Events
        {
            get => (IReadOnlyCollection<IKeyEvent>)_events;
            internal set => _events = value as ICollection<IKeyEvent> ?? throw new Exception(
                $"The collection should be {nameof(ICollection<IKeyEvent>)}");
        }

        public bool EnableGroupedSerialization { get; set; }/* = true;*/

        public double StartTime
        {
            get => _startTime;
            set
            {
                if (Precision.AlmostEquals(_startTime, value)) return;
                _startTime = value;
                ResetCacheAndRaiseTimingChanged();
            }
        }

        public double EndTime
        {
            get => OuterMaxTime();
            set => throw new NotSupportedException();
        }

        public int LoopCount { get; set; }
        public double OuterMaxTime() => StartTime + MaxTime() * LoopCount;
        public double OuterMinTime() => StartTime + MinTime();
        public double MaxTime()
        {
            if (_cachedMaxTime != null) return _cachedMaxTime.Value;
            return (double)(_cachedMaxTime = Events.Count > 0 ? Events.Max(k => k.EndTime) : 0);
        }

        public double MinTime()
        {
            if (_cachedMinTime != null) return _cachedMinTime.Value;
            return (double)(_cachedMinTime = Events.Count > 0 ? Events.Min(k => k.StartTime) : 0);
        }

        public double MaxStartTime()
        {
            if (_cachedMaxStartTime != null) return _cachedMaxStartTime.Value;
            return (double)(_cachedMaxStartTime = Events.Count > 0 ? Events.Max(k => k.StartTime) : 0);
        }

        public double MinEndTime()
        {
            if (_cachedMinEndTime != null) return _cachedMinEndTime.Value;
            return (double)(_cachedMinEndTime = Events.Count > 0 ? Events.Min(k => k.EndTime) : 0);
        }

        public Loop(double startTime, int loopCount)
        {
            StartTime = startTime;
            LoopCount = loopCount;
        }

        public async Task WriteHeaderAsync(TextWriter writer)
        {
            await writer.WriteAsync(EventType.Flag);
            await writer.WriteAsync(',');
            await writer.WriteAsync(Math.Round(StartTime));
            await writer.WriteAsync(',');
            await writer.WriteAsync(LoopCount);
        }

        public async Task WriteScriptAsync(TextWriter writer)
        {
            await ScriptHelper.WriteSubEventHostAsync(writer, this, EnableGroupedSerialization);
        }

        public void AdjustTiming(double offset)
        {
            StartTime += offset;
        }

        IReadOnlyCollection<IKeyEvent> IEventHost.Events => Events;

        public void AddEvent(IKeyEvent @event)
        {
            _events.Add(@event);
            @event.TimingChanged += ResetCacheAndRaiseTimingChanged;
            ResetCacheAndRaiseTimingChanged();
        }

        public bool RemoveEvent(IKeyEvent @event)
        {
            @event.TimingChanged -= ResetCacheAndRaiseTimingChanged;
            var removeEvent = _events.Remove(@event);
            if (removeEvent)
                ResetCacheAndRaiseTimingChanged();
            return removeEvent;
        }

        public void ClearEvents(IComparer<IKeyEvent>? comparer = null)
        {
            var valid = _events.Count > 0;
            foreach (var @event in _events)
                @event.TimingChanged -= ResetCacheAndRaiseTimingChanged;
            _events.Clear();
            if (comparer == null)
                _events = new HashSet<IKeyEvent>();
            else
                _events = new SortedSet<IKeyEvent>(comparer);
            if (valid) ResetCacheAndRaiseTimingChanged();
        }

        ISceneObject? ISubEventHost.BaseObject
        {
            get => _baseObject;
            set => _baseObject = value;
        }

        public object Clone()
        {
            var loop = new Loop(StartTime, LoopCount)
            {
                EnableGroupedSerialization = EnableGroupedSerialization
            };

            foreach (var keyEvent in _events)
                loop.AddEvent((IKeyEvent)keyEvent.Clone());

            return loop;
        }

        private void ResetCacheAndRaiseTimingChanged()
        {
            _cachedMaxTime = null;
            _cachedMinTime = null;
            _cachedMaxStartTime = null;
            _cachedMinEndTime = null;
            TimingChanged?.Invoke();
        }
    }
}
