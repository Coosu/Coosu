using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coosu.Shared;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Events
{
    public sealed class Trigger : ISubEventHost, IEvent
    {
        internal ISceneObject? _baseObject;
        private const string HitSound = "HitSound";

        public EventType EventType { get; } = EventTypes.Trigger;

        public bool EnableGroupedSerialization { get; set; } /*= true;*/
        public ICollection<IKeyEvent> Events { get; set; } =
            new SortedSet<IKeyEvent>(new EventSequenceComparer());

        public double StartTime { get; set; }
        public double EndTime { get; set; }
        public string TriggerName { get; set; }

        public double MaxTime =>
            EndTime +
            (Events.Count > 0
                ? Events.Max(k => k.EndTime)
                : 0);

        public double MinTime => StartTime;

        public double MaxStartTime =>
            EndTime +
            (Events.Count > 0
                ? Events.Max(k => k.StartTime)
                : 0); //if hitsound played at end time

        public double MinEndTime => StartTime; // if no hitsound here

        public Trigger(double startTime, double endTime, TriggerType triggerType, bool listenSample = false, uint? customSampleSet = null)
        {
            StartTime = startTime;
            EndTime = endTime;

            TriggerName = GetTriggerString(triggerType, listenSample, customSampleSet);
        }

        public Trigger(double startTime, double endTime, string triggerName)
        {
            StartTime = startTime;
            EndTime = endTime;
            TriggerName = triggerName;
        }

        public async Task WriteHeaderAsync(TextWriter writer)
        {
            await writer.WriteAsync(EventType.Flag);
            await writer.WriteAsync(',');
            await writer.WriteAsync(TriggerName);
            await writer.WriteAsync(',');
            await writer.WriteAsync(Math.Round(StartTime));
            await writer.WriteAsync(',');
            await writer.WriteAsync(Math.Round(EndTime));
        }

        public async Task WriteScriptAsync(TextWriter writer)
        {
            await ScriptHelper.WriteSubEventHostAsync(writer, this, EnableGroupedSerialization);
        }

        private static string GetTriggerString(TriggerType triggerType, bool listenSample, uint? customSampleSet)
        {
            var sb = new StringBuilder(HitSound, 23);
            if (triggerType.HasFlag(TriggerType.HitSoundNormal) ||
                triggerType.HasFlag(TriggerType.HitSoundSoft) ||
                triggerType.HasFlag(TriggerType.HitSoundDrum))
            {
                if (listenSample)
                    sb.Append("All");

                if (triggerType.HasFlag(TriggerType.HitSoundNormal))
                    sb.Append("Normal");
                else if (triggerType.HasFlag(TriggerType.HitSoundSoft))
                    sb.Append("Soft");
                else if (triggerType.HasFlag(TriggerType.HitSoundDrum))
                    sb.Append("Drum");

                if (listenSample)
                {
                    var str = sb.ToString();
                    return str.EndsWith("All") ? HitSound : str;
                }
            }

            if (triggerType.HasFlag(TriggerType.HitSoundWhistle) ||
                triggerType.HasFlag(TriggerType.HitSoundFinish) ||
                triggerType.HasFlag(TriggerType.HitSoundClap))
            {
                if (triggerType.HasFlag(TriggerType.HitSoundWhistle))
                    sb.Append("Whistle");
                else if (triggerType.HasFlag(TriggerType.HitSoundFinish))
                    sb.Append("Finish");
                else if (triggerType.HasFlag(TriggerType.HitSoundClap))
                    sb.Append("Clap");
            }

            if (customSampleSet != null) sb.Append(customSampleSet.ToString());
            return sb.ToString();
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
            var trigger = new Trigger(StartTime, EndTime, TriggerName)
            {
                Events = Events.Select(k => k.Clone()).Cast<IKeyEvent>().ToList(),
                EnableGroupedSerialization = EnableGroupedSerialization
            };
            return trigger;
        }
    }
}
