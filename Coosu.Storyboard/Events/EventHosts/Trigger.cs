using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Events.EventHosts
{
    public sealed class Trigger : ISubEventHost, IEvent
    {
        public EventType EventType { get; } = EventTypes.Trigger;

        internal ISceneObject? _baseObject;
        private const string HitSound = "HitSound";
        public string Header => $"T,{TriggerName},{StartTime},{EndTime}";
        public bool EnableGroupedSerialization { get; set; }
        public SortedSet<ICommonEvent> Events { get; } = new(new EventTimingComparer());

        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public string TriggerName { get; set; }

        public float MaxTime =>
            EndTime +
            (Events.Count > 0
                ? Events.Max(k => k.EndTime)
                : 0);

        public float MinTime => StartTime;

        public float MaxStartTime =>
            EndTime +
            (Events.Count > 0
                ? Events.Max(k => k.StartTime)
                : 0); //if hitsound played at end time

        public float MinEndTime => StartTime; // if no hitsound here

        public Trigger(float startTime, float endTime, TriggerType triggerType, bool listenSample = false, uint? customSampleSet = null)
        {
            StartTime = startTime;
            EndTime = endTime;

            TriggerName = GetTriggerString(triggerType, listenSample, customSampleSet);
        }

        public Trigger(float startTime, float endTime, string triggerName)
        {
            StartTime = startTime;
            EndTime = endTime;
            TriggerName = triggerName;
        }

        public async Task WriteScriptAsync(TextWriter writer)
        {
            await ScriptHelper.WriteTriggerAsync(writer, this, EnableGroupedSerialization);
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
