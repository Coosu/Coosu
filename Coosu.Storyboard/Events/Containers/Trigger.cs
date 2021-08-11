using Coosu.Storyboard.Utils;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coosu.Storyboard.Events.Containers
{
    public sealed class Trigger : EventContainer, IEvent
    {
        protected override string Header => $"T,{TriggerName},{StartTime},{EndTime}";
        private const string HitSound = "HitSound";

        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public string TriggerName { get; set; }

        public override float MaxTime =>
            EndTime +
            (EventList.Count > 0
                ? EventList.Max(k => k.EndTime)
                : 0);

        public override float MinTime => StartTime;

        public override float MaxStartTime =>
            EndTime +
            (EventList.Count > 0
                ? EventList.Max(k => k.StartTime)
                : 0); //if hitsound played at end time

        public override float MinEndTime => StartTime; // if no hitsound here

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

        public override void Adjust(float offsetX, float offsetY, int offsetTiming)
        {
            StartTime += offsetTiming;
            EndTime += offsetTiming;
            base.Adjust(offsetX, offsetY, offsetTiming);
        }

        public override async Task WriteScriptAsync(TextWriter sb)
        {
            await sb.WriteTriggerAsync(this, group);
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
    }
}
