using System;
using System.IO;
using System.Threading.Tasks;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Events
{
    public abstract class CommonEvent : ICommonEvent,
        IScriptable,
        IAdjustableTimingEvent
    //,IComparable<CommonEvent>
    {
        public abstract EventType EventType { get; }
        public EasingType Easing { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public float[] Start { get; set; }
        public float[] End { get; set; }

        public virtual int ParamLength => Start.Length;
        public virtual bool IsStatic => Start.Equals(End);

        //public int CompareTo(CommonEvent? other)
        //{
        //    if (other == null)
        //        return 1;
        //    if (StartTime > other.StartTime)
        //        return 1;
        //    if (StartTime.Equals(other.StartTime))
        //        return 0;
        //    if (StartTime < other.StartTime)
        //        return -1;
        //    throw new ArgumentOutOfRangeException(nameof(other));
        //}

        public void AdjustTiming(float time)
        {
            StartTime += time;
            EndTime += time;
        }

        public virtual async Task WriteScriptAsync(TextWriter writer)
        {
            string e = EventType.ToShortString();
            string easing = ((int)Easing).ToString();
            string startT = Math.Round(StartTime).ToIcString();
            string endT = StartTime.Equals(EndTime)
                ? ""
                : Math.Round(EndTime).ToIcString();
            await writer.WriteAsync($"{e},{easing},{startT},{endT},");
            await WriteExtraScriptAsync(writer);
        }
        protected CommonEvent()
        {
            Start = Array.Empty<float>();
            End = Array.Empty<float>();
        }

        protected CommonEvent(EasingType easing, float startTime, float endTime, float[] start, float[] end)
        {
            Easing = easing;
            StartTime = startTime;
            EndTime = endTime;
            Start = start;
            End = end;
        }

        protected virtual async Task WriteExtraScriptAsync(TextWriter textWriter)
        {
            bool sequenceEqual = true;
            int count = Start.Length;
            for (int i = 0; i < count; i++)
            {
                if (Start[i].Equals(End[i]))
                {
                    sequenceEqual = false;
                    break;
                }
            }

            if (sequenceEqual)
                await WriteStartAsync(textWriter, count);
            else
                await WriteFullAsync(textWriter, count);
        }

        private async Task WriteStartAsync(TextWriter textWriter, int count)
        {
            for (int i = 0; i < count; i++)
            {
                await textWriter.WriteAsync(Start[i].ToIcString());
                if (i != count - 1) await textWriter.WriteAsync(',');
            }
        }

        private async Task WriteFullAsync(TextWriter textWriter, int count)
        {
            for (int i = 0; i < count; i++)
            {
                await textWriter.WriteAsync(Start[i].ToIcString());
                await textWriter.WriteAsync(',');
                await textWriter.WriteAsync(End[i].ToIcString());
                if (i != count - 1) await textWriter.WriteAsync(',');
            }
        }
    }
}
