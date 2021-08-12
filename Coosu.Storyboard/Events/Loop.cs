using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Events
{
    public sealed class Loop : ISubEventHost, IEvent
    {
        public EventType EventType { get; } = EventTypes.Loop;

        internal ISceneObject? _baseObject;
        public string Header => $"L,{StartTime},{LoopCount}";
        public bool EnableGroupedSerialization { get; set; }
        public SortedSet<ICommonEvent> Events { get; } = new(new EventTimingComparer());

        public float StartTime { get; set; }
        public float EndTime
        {
            get => OuterMaxTime;
            set => throw new System.NotSupportedException();
        }

        public int LoopCount { get; set; }
        public float OuterMaxTime => StartTime + MaxTime * LoopCount;
        public float OuterMinTime => StartTime + MinTime;
        public float MaxTime => Events.Count > 0 ? Events.Max(k => k.EndTime) : 0;
        public float MinTime => Events.Count > 0 ? Events.Min(k => k.StartTime) : 0;
        public float MaxStartTime => Events.Count > 0 ? Events.Max(k => k.StartTime) : 0;
        public float MinEndTime => Events.Count > 0 ? Events.Min(k => k.EndTime) : 0;

        public Loop(float startTime, int loopCount)
        {
            StartTime = startTime;
            LoopCount = loopCount;
        }

        public async Task WriteScriptAsync(TextWriter writer)
        {
            await ScriptHelper.WriteLoopAsync(writer, this, EnableGroupedSerialization);
        }

        public void AdjustTiming(float offset)
        {
            StartTime += offset;
        }

        public void AddEvent(ICommonEvent @event)
        {
            Events.Add(@event);
        }

        ISceneObject? ISubEventHost.BaseObject
        {
            get => _baseObject;
            set => _baseObject = value;
        }
    }
}
