using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Shared;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Events
{
    public sealed class Loop : ISubEventHost, IEvent
    {
        public EventType EventType { get; } = EventTypes.Loop;

        internal ISceneObject? _baseObject;
        public bool EnableGroupedSerialization { get; set; }/* = true;*/
        public ICollection<IKeyEvent> Events { get; set; } =
            new SortedSet<IKeyEvent>(new EventTimingComparer());

        public double StartTime { get; set; }
        public double EndTime
        {
            get => OuterMaxTime;
            set => throw new System.NotSupportedException();
        }

        public int LoopCount { get; set; }
        public double OuterMaxTime => StartTime + MaxTime * LoopCount;
        public double OuterMinTime => StartTime + MinTime;
        public double MaxTime => Events.Count > 0 ? Events.Max(k => k.EndTime) : 0;
        public double MinTime => Events.Count > 0 ? Events.Min(k => k.StartTime) : 0;
        public double MaxStartTime => Events.Count > 0 ? Events.Max(k => k.StartTime) : 0;
        public double MinEndTime => Events.Count > 0 ? Events.Min(k => k.EndTime) : 0;

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

        public void AddEvent(IKeyEvent @event)
        {
            Events.Add(@event);
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
                Events = Events.Select(k => k.Clone()).Cast<IKeyEvent>().ToList(),
                EnableGroupedSerialization = EnableGroupedSerialization
            };
            return loop;
        }
    }
}
