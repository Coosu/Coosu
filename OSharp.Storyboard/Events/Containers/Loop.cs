﻿using OSharp.Storyboard.Internal;
using System.IO;
using System.Linq;

namespace OSharp.Storyboard.Events.Containers
{
    public sealed class Loop : EventContainer, IEvent
    {
        protected override string Head => $"L,{StartTime},{LoopCount}";

        public float StartTime { get; set; }
        public float EndTime => OuterMaxTime;

        public int LoopCount { get; set; }
        public float OuterMaxTime => StartTime + MaxTime * LoopCount;
        public float OuterMinTime => StartTime + MinTime;
        public override float MaxTime => EventList.Count > 0 ? EventList.Max(k => k.EndTime) : 0;
        public override float MinTime => EventList.Count > 0 ? EventList.Min(k => k.StartTime) : 0;
        public override float MaxStartTime => EventList.Count > 0 ? EventList.Max(k => k.StartTime) : 0;
        public override float MinEndTime => EventList.Count > 0 ? EventList.Min(k => k.EndTime) : 0;

        public Element BaseElement { get; internal set; }

        public Loop(float startTime, int loopCount)
        {
            StartTime = startTime;
            LoopCount = loopCount;
        }

        public override void Adjust(float offsetX, float offsetY, int offsetTiming)
        {
            StartTime += offsetTiming;
            base.Adjust(offsetX, offsetY, offsetTiming);
        }

        public override void WriteOsbString(TextWriter sb, bool group = false)
        {
            sb.WriteLoop(this, group);
        }

        public override string ToString() => Head;
    }
}
