using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Extensibility;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard.Events
{
    public abstract class CommonEvent : ICommonEvent,
            IScriptable
    //,IComparable<CommonEvent>
    {
        public abstract EventType EventType { get; }
        public EasingType Easing { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public float[] Start { get; set; }
        public float[] End { get; set; }

        public virtual int ParamLength => Start.Length;
        public virtual bool IsStatic => Start.SequenceEqual(End);

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

        public void AdjustTiming(float offset)
        {
            StartTime += offset;
            EndTime += offset;
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

        public static ICommonEvent Create(EventType e, EasingType easing,
            float startTime, float endTime, float[] start, float[]? end)
        {
            ICommonEvent commonEvent;
            if (end == null || end.Length == 0)
                end = start;

            if (e == EventTypes.Fade)
            {
                commonEvent = new Fade(easing, startTime, endTime, start[0], end[0]);
            }
            else if (e == EventTypes.Move)
            {
                commonEvent = new Move(easing, startTime, endTime, start[0], start[1], end[0], end[1]);
            }
            else if (e == EventTypes.MoveX)
            {
                commonEvent = new MoveX(easing, startTime, endTime, start[0], end[0]);
            }
            else if (e == EventTypes.MoveY)
            {
                commonEvent = new MoveY(easing, startTime, endTime, start[0], end[0]);
            }
            else if (e == EventTypes.Scale)
            {
                commonEvent = new Scale(easing, startTime, endTime, start[0], end[0]);
            }
            else if (e == EventTypes.Vector)
            {
                commonEvent = new Vector(easing, startTime, endTime, start[0], start[1], end[0], end[1]);
            }
            else if (e == EventTypes.Rotate)
            {
                commonEvent = new Rotate(easing, startTime, endTime, start[0], end[0]);
            }
            else if (e == EventTypes.Color)
            {
                commonEvent = new Color(easing, startTime, endTime, start[0], start[1], start[2], end[0], end[1],
                    end[2]);
            }
            else if (e == EventTypes.Parameter)
            {
                commonEvent = new Parameter(easing, startTime, endTime, (ParameterType)(int)start[0]);
            }
            else
            {
                var result = HandlerRegister.GetEventTransformation(e)?.Invoke(e, easing, startTime, endTime, start, end);
                commonEvent = result ?? throw new ArgumentOutOfRangeException(nameof(e), e, null);
            }

            return commonEvent;
        }
    }
}
