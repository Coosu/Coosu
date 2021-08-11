using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Events.EventHosts
{
    public sealed class Loop : ISubEventHost, IEvent
    {
        internal ISceneObject? _baseObject;
        public string Header => $"L,{StartTime},{LoopCount}";
        public bool EnableGroupedSerialization { get; set; }
        public SortedSet<CommonEvent> Events { get; } = new(new EventTimingComparer());

        public float StartTime { get; set; }
        public float EndTime => OuterMaxTime;

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

        public async Task WriteScriptAsync(TextWriter sb)
        {
            await sb.WriteLoopAsync(this, EnableGroupedSerialization);
        }

        public void AdjustTiming(float offset)
        {
            StartTime += offset;
        }

        ISceneObject? ISubEventHost.BaseObject
        {
            get => _baseObject;
            set => _baseObject = value;
        }
    }
}
